using AutoMapper;
using FFMpegUI.Models;
using FFMpegUI.Persistence.Entities;
using FFMpegUI.Persistence.Repositories;
using FFMpegUI.Services.Configuration;
using PagedList.Core;

namespace FFMpegUI.Services
{
    public class FFMpegService : IFFMpegManagementService, IFFMpegConvertingService
    {
        private readonly IMapper mapper;
        private readonly IFFMpegProcessFeaturesRepository processFeaturesRepository;
        private readonly IFFMpegProcessRepository processRepository;
        private readonly IFFMpegProcessItemsRepository processItemsRepository;
        private readonly FFMpegUIServiceConfiguration configuration;

        public FFMpegService(IMapper mapper,
            IFFMpegProcessFeaturesRepository processFeaturesRepository,
            IFFMpegProcessRepository processRepository,
            IFFMpegProcessItemsRepository processItemsRepository,
            FFMpegUIServiceConfiguration configuration)
        {
            this.mapper = mapper;
            this.processFeaturesRepository = processFeaturesRepository;
            this.processRepository = processRepository;
            this.processItemsRepository = processItemsRepository;
            this.configuration = configuration;
        }

        async Task IFFMpegManagementService.CreateProcess(FFMpegCreateProcessCommand command)
        {
            var submissionDate = DateTime.Now;

            var filesModel = new List<FFMpegProcessItem>();

            // save each file to the designated source directory
            var newProcessFolderGuid = Guid.NewGuid().ToString();
            var sourceFilesBasePath = Path.Combine(configuration.SourceFilesDirectoryPath, newProcessFolderGuid);
            Directory.CreateDirectory(sourceFilesBasePath);

            foreach(var f in command.Files)
            {
                var assignedFileName = Guid.NewGuid().ToString();
                var assignedFileFullPath = Path.Combine(sourceFilesBasePath, assignedFileName);
                using (var writeStream = new FileStream(assignedFileFullPath, FileMode.Create, FileAccess.Write)) 
                {  
                    await f.UpcomingStream.CopyToAsync(writeStream);
                }

                filesModel.Add(new FFMpegProcessItem
                {
                     SourceFileFullPath = assignedFileFullPath                     
                });
            }

            var newProcess = new FFMpegProcess
            {
                SubmissionDate = submissionDate,
                 Items = filesModel,
                  
            };

            var eProcess = mapper.Map<FFMpegPersistedProcess>(newProcess);
            var eProcessItems = mapper.Map<IEnumerable<FFMpegPersistedProcessItem>>(newProcess.Items);
            var eProcessFeatures = mapper.Map<FFMpegPersistedProcessFeatures>(newProcess);

            var transactionId = await processRepository.BeginTransactionAsync();
            try
            {
                var persistedProcess = await processRepository.CreateAsync(eProcess);
                var persistedProcessId = persistedProcess.Id;

                foreach (var eProcessItem in eProcessItems)
                {
                    eProcessItem.ProcessId = persistedProcessId;
                }
                var persistedProcessItems = await processItemsRepository.CreateAsync(eProcessItems);

                var persistedProcessFeatures = await processFeaturesRepository.CreateAsync(eProcessFeatures);

                await processRepository.ConfirmTransactionAsync(transactionId);
            }
            catch
            {
                await processRepository.RejectTransactionAsync(transactionId);
                throw;
            }
        }


        private async Task<FFMpegProcess> GetProcessAndItems(int processId)
        {
            var eProcess = await processRepository.GetWithItemsAsync(processId);
            var eProcessFeatures = await processFeaturesRepository.GetAsync(processId);
            var ret = mapper.Map<FFMpegProcess>((eProcess, eProcessFeatures));
            return ret;
        }

        async Task<IPagedList<FFMpegProcessSummary>> IFFMpegManagementService.GetProcessesSummary(int pageNumber, int pageSize)
        {
            var ret = await processRepository.GetAllSummaryAsync(pageNumber, pageSize);
            return ret;            
        }
    }
}
