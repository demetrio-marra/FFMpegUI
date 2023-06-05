using AutoMapper;
using FFMpegUI.Models;
using FFMpegUI.Persistence.Entities;
using FFMpegUI.Persistence.Repositories;
using FFMpegUI.Services.Configuration;
using FFMpegUI.Services.Middlewares;
using PagedList.Core;

namespace FFMpegUI.Services
{
    public class FFMpegService : IFFMpegManagementService, IFFMpegConvertingService
    {
        private readonly IMapper mapper;
        private readonly IFFMpegProcessFeaturesRepository processFeaturesRepository;
        private readonly IFFMpegProcessRepository processRepository;
        private readonly IFFMpegProcessItemsRepository processItemsRepository;
        private readonly IQFileServerApiService qFileServerApiService;
        private readonly FFMpegUIServiceConfiguration configuration;

        public FFMpegService(IMapper mapper,
            IFFMpegProcessFeaturesRepository processFeaturesRepository,
            IFFMpegProcessRepository processRepository,
            IFFMpegProcessItemsRepository processItemsRepository,
            IQFileServerApiService qFileServerApiService,
            FFMpegUIServiceConfiguration configuration)
        {
            this.mapper = mapper;
            this.processFeaturesRepository = processFeaturesRepository;
            this.processRepository = processRepository;
            this.processItemsRepository = processItemsRepository;
            this.qFileServerApiService = qFileServerApiService;
            this.configuration = configuration;
        }

        void IFFMpegConvertingService.BeginConvert(int processId)
        {
            throw new NotImplementedException();
        }

        async Task IFFMpegManagementService.CreateProcess(FFMpegCreateProcessCommand command)
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
                    SourceFileId = uploadedFile.Id
                });
            }

            var newProcess = new FFMpegProcess
            {
                SubmissionDate = submissionDate,
                Items = processItemsList,
                AudioCodec = command.AudioCodec,
                OverallConversionQuality = command.OverallConversionQuality,
                VideoCodec = command.VideoCodec,
                RescaleHorizontalWidth = command.RescaleHorizontalWidth                
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

                await processRepository.ConfirmTransactionAsync(transactionId);
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
    }
}
