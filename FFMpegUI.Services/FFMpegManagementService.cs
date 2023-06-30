using AutoMapper;
using FFMpegUI.Infrastructure.Services;
using FFMpegUI.Infrastructure.Support;
using FFMpegUI.Messages;
using FFMpegUI.Models;
using FFMpegUI.Persistence.Entities;
using FFMpegUI.Persistence.Repositories;
using FFMpegUI.Services.Middlewares;
using PagedList.Core;

namespace FFMpegUI.Services
{
    public class FFMpegManagementService : IFFMpegManagementService
    {
        private readonly IMapper mapper;
        private readonly IFFMpegProcessFeaturesRepository processFeaturesRepository;
        private readonly IFFMpegProcessRepository processRepository;
        private readonly IFFMpegProcessItemsRepository processItemsRepository;
        private readonly IQFileServerApiService qFileServerApiService;
        private readonly IFFMpegUIApiService apiService;
        private readonly BackgroundTaskQueue taskQueue;
        private readonly IPresentationUpdater presentationUpdater;


        public FFMpegManagementService(IMapper mapper,
            IFFMpegProcessFeaturesRepository processFeaturesRepository,
            IFFMpegProcessRepository processRepository,
            IFFMpegProcessItemsRepository processItemsRepository,
            IQFileServerApiService qFileServerApiService,
            IFFMpegUIApiService apiService,
            BackgroundTaskQueue taskQueue,
            IPresentationUpdater presentationUpdater
            )
        {
            this.mapper = mapper;
            this.processFeaturesRepository = processFeaturesRepository;
            this.processRepository = processRepository;
            this.processItemsRepository = processItemsRepository;
            this.qFileServerApiService = qFileServerApiService;
            this.apiService = apiService;
            this.taskQueue = taskQueue;
            this.presentationUpdater = presentationUpdater;
        }


        async Task<FFMpegProcess> IFFMpegManagementService.CreateProcess(FFMpegCreateProcessCommand command)
        {
            var submissionDate = DateTime.Now;

            var processItemsList = new List<FFMpegProcessItem>();

            foreach(var f in command.Files)
            {
                // upload each file to QFileServer and get its registered id
                var uploadedFile = await qFileServerApiService.UploadFile(f.UpcomingStream, f.UpcomingFileName);
                
                processItemsList.Add(new FFMpegProcessItem
                {
                    SourceFileName = f.UpcomingFileName,
                    SourceFileId = uploadedFile.Id,
                    SourceFileSize = uploadedFile.Size
                });
            }

            var newProcess = new FFMpegProcess
            {
                SubmissionDate = submissionDate,
                Items = processItemsList,
                AudioCodec = command.AudioCodec,
                OverallConversionQuality = command.OverallConversionQuality,
                VideoCodec = command.VideoCodec,
                RescaleHorizontalWidth = command.RescaleHorizontalWidth     ,
                SourceFilesTotalSize = processItemsList.Sum(f => f.SourceFileSize)
            };

            var eProcess = mapper.Map<FFMpegPersistedProcess>(newProcess);
            var eProcessItems = mapper.Map<IEnumerable<FFMpegPersistedProcessItem>>(newProcess.Items);
            var eProcessFeatures = mapper.Map<FFMpegPersistedProcessFeatures>(newProcess);

            var transactionId = await processRepository.BeginTransactionAsync();
            try
            {
                var persistedProcess = await processRepository.CreateAsync(eProcess);
                var persistedProcessId = persistedProcess.Id;
                eProcessFeatures.ProcessId = persistedProcessId;

                foreach (var eProcessItem in eProcessItems)
                {
                    eProcessItem.ProcessId = persistedProcessId;
                }
                var persistedProcessItems = await processItemsRepository.CreateAsync(eProcessItems);

                await processFeaturesRepository.CreateAsync(eProcessFeatures);

                eProcess.StartDate = DateTime.Now;
                await processRepository.UpdateAsync(eProcess);

                await processRepository.ConfirmTransactionAsync(transactionId);

                taskQueue.Enqueue(persistedProcessId); // actually start the conversion process on a background longrunning task

                var ret = await GetProcessAndItems(persistedProcessId);
                return ret;
            }
            catch
            {
                await processRepository.RejectTransactionAsync(transactionId);
                throw;
            }
        }


        async Task<FFMpegFileDownloadDTO> IFFMpegManagementService.GetFileForDownload(long fileId)
        {
            var x = await qFileServerApiService.DownloadFile(fileId);
            var ret = new FFMpegFileDownloadDTO
            {
                ContentType = x.ContentType,
                FileName = x.FileName,
                FileStream = x.FileStream
            };

            return ret;
        }


        private async Task<FFMpegProcess> GetProcessAndItems(int processId)
        {
            var eProcess = await processRepository.GetWithItemsAsync(processId);
            var eProcessFeatures = await processFeaturesRepository.GetAsync(processId);
            var ret = mapper.Map<FFMpegProcess>((eProcess, eProcessFeatures));
            return ret;
        }


        async Task<FFMpegProcess> IFFMpegManagementService.GetProcessDetails(int processId)
        {
            var ret = await GetProcessAndItems(processId);
            return ret;
        }


        async Task<IPagedList<FFMpegProcessSummary>> IFFMpegManagementService.GetProcessesSummary(int pageNumber, int pageSize)
        {
            var ret = await processRepository.GetAllSummaryAsync(pageNumber, pageSize);
            return ret;            
        }


        async Task IFFMpegManagementService.PerformProcessConversion(int processId)
        {
            var processo = await GetProcessAndItems(processId);

            var parametriConversione = new FFMpegConvertParameters
            {
                AudioCodec = processo.AudioCodec,
                OverallConversionQuality = processo.OverallConversionQuality,
                RescaleHorizontalWidth = processo.RescaleHorizontalWidth,
                VideoCodec = processo.VideoCodec
            };

            foreach (var item in processo.Items)
            {
                var processItemId = item.Id;

                var sFileId = item.SourceFileId!.Value;

                try
                {
                    // chiamo il servizio di conversione passandogli l'id del file su QFileServer
                    // Lo stesso servizio caricherà su QFileServer il file convertito e ne restituirà l'id qui
                    await apiService.ConvertEmittingMessages(processId, sFileId, processItemId, parametriConversione);

                    // lo stato del processItem e di quello principale sarà aggiornato dall'api progress
                }
                catch (Exception ex)
                {
                    var endDate = DateTime.Now;
                    await ManageJobSubmissionFailure(processId, processItemId, endDate, ex.Message);
                }
            }            
        }


        async Task IFFMpegManagementService.ElaborateProcessItemProgressMessage(FFMpegProcessItemMessage message)
        {
            var processItemUpdateCommand = mapper.Map<FFMpegUpdateProcessItemCommand>(message);

            var mainProcessUpdatedToo = false;

            var tran = await processItemsRepository.BeginTransactionAsync();

            try
            {
                await processItemsRepository.UpdateProgressInfo(processItemUpdateCommand);

                // TODO: update process start date

                // check if the whole process status should change
                if (message.Successfull.HasValue)
                {
                    var processId = message.ProcessId;

                    var processUpdateCommand = new FFMpegUpdateProcessCommand
                    {
                        ProcessId = processId
                    };

                    var process = await processRepository.GetWithItemsAsync(processId);

                    if (process.Items.All(i => i.Successfull.HasValue))
                    {                        
                        var allSuccess = process.Items.All(i => i.Successfull == true);
                        var statusMessage = allSuccess ? "done" : "Failed item (" + processItemUpdateCommand.StatusMessage + ")";

                        processUpdateCommand.Successfull = allSuccess;
                        processUpdateCommand.EndDate = processItemUpdateCommand.EndDate;
                        processUpdateCommand.StatusMessage = statusMessage;
                        processUpdateCommand.StartDate = process.Items.Min(i => i.StartDate); // temporaneo
                    }

                    processUpdateCommand.ConvertedFilesTotalSize = process.Items.Sum(i => i.ConvertedFileSize);

                    await processRepository.UpdateProgressInfo(processUpdateCommand);
                    mainProcessUpdatedToo = true;
                }

                await processItemsRepository.ConfirmTransactionAsync(tran);
            } 
            catch
            {
                await processItemsRepository.RejectTransactionAsync(tran);
            }

            // send to browser via SignalR
            await DispatchProcessItemStatusNotification(message.ProcessItemId);

            if (mainProcessUpdatedToo)
            {
                await DispatchProcessStatusNotification(message.ProcessId);
            }
        }


        private async Task DispatchProcessItemStatusNotification(int processItemId)
        {
            var processItem = await GetProcessItemAsync(processItemId);
            if (processItem == null)
            {
                return;
            }

            var processItemStatusNotification = mapper.Map<FFMpegProcessItemStatusNotification>(processItem);
            await presentationUpdater.UpdateProcessItem(processItemStatusNotification);
        }


        private async Task DispatchProcessStatusNotification(int processId)
        {
            var process = await GetProcessAsync(processId);
            if (process == null)
            {
                return;
            }

            var processStatusNotification = mapper.Map<FFMpegProcessStatusNotification>(process);
            await presentationUpdater.UpdateProcess(processStatusNotification);
        }


        private async Task ManageJobSubmissionFailure(int processId, int processItemId, DateTime failureTimestamp, string failureReason)
        {
            // aggiorna item
            var processItemUpdateCommand = new FFMpegUpdateProcessItemCommand
            {
                EndDate = failureTimestamp,
                ProcessItemId = processItemId,
                Successfull = false,
                StatusMessage = failureReason
            };

            var processUpdateCommand = new FFMpegUpdateProcessCommand
            {
                EndDate = failureTimestamp,
                ProcessId = processId,
                Successfull = false,
                StatusMessage = failureReason
            };

            var tranId = await processItemsRepository.BeginTransactionAsync();

            try
            {
                await processItemsRepository.UpdateProgressInfo(processItemUpdateCommand);

                // aggiorna processo principale
                await processRepository.UpdateProgressInfo(processUpdateCommand);

                await processItemsRepository.ConfirmTransactionAsync(tranId);
            }
            catch
            {
                await processItemsRepository.RejectTransactionAsync(tranId);
                return;
            }

            await DispatchProcessItemStatusNotification(processItemId);
            await DispatchProcessStatusNotification(processId);
        }


        private async Task<FFMpegProcessItem> GetProcessItemAsync(int processItemId)
        {
            var entity = await processItemsRepository.GetAsync(processItemId);
            var ret = mapper.Map<FFMpegProcessItem>(entity);
            return ret;
        }

        private async Task<FFMpegProcess> GetProcessAsync(int processId)
        {
            var entity = await processRepository.GetAsync(processId);
            var ret = mapper.Map<FFMpegProcess>(entity);
            return ret;
        }
    }
}
