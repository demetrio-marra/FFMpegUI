using AutoMapper;
using FFMpegUI.Models;
using FFMpegUI.Persistence.Repositories;
using FFMpegUI.Services.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FFMpegUI.Infrastructure.Support
{
    public class ConvertProcessTaskRunner : BackgroundService
    {
        private readonly BackgroundTaskQueue taskQueue;

        private readonly IServiceProvider _serviceProvider;


        public ConvertProcessTaskRunner(BackgroundTaskQueue taskQueue,
            IServiceProvider serviceProvider
            )
        {
            this.taskQueue = taskQueue;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var processId = await taskQueue.DequeueAsync();
                await ConvertJob(processId);
            }
        }

        private async Task ConvertJob(int processId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var qFileServerApiService = scope.ServiceProvider.GetRequiredService<IQFileServerApiService>();
                var processFeaturesRepository = scope.ServiceProvider.GetRequiredService<IFFMpegProcessFeaturesRepository>();
                var processRepository = scope.ServiceProvider.GetRequiredService<IFFMpegProcessRepository>();
                var processItemsRepository = scope.ServiceProvider.GetRequiredService<IFFMpegProcessItemsRepository>();
                var apiService = scope.ServiceProvider.GetRequiredService<IFFMpegUIApiService>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                await ConvertJobPayload(processId, 
                    qFileServerApiService, 
                    processFeaturesRepository, 
                    processRepository, 
                    processItemsRepository, 
                    apiService,
                    mapper);
            }
        }


        private async Task ConvertJobPayload(int processId,
            IQFileServerApiService qFileServerApiService,
            IFFMpegProcessFeaturesRepository processFeaturesRepository,
            IFFMpegProcessRepository processRepository,
            IFFMpegProcessItemsRepository processItemsRepository,
            IFFMpegUIApiService apiService,
            IMapper mapper)
        {
            var processo = await GetProcessAndItems(processId, 
                processRepository,
                processFeaturesRepository,
                mapper);

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

                await processItemsRepository.UpdateStartInfo(processItemId, DateTime.Now);

                var successfull = false;
                long? convertedFileId = null;
                string? convertedFileName = null;
                string? errorMessage = null;
                DateTime? endDate;

                try
                {
                    // chiamo il servizio di conversione passandogli l'id del file su QFileServer
                    // Lo stesso servizio caricherà su QFileServer il file convertito e ne restituirà l'id qui
                    var convertedFileDTO = await apiService.Convert(sFileId, parametriConversione);

                    successfull = true;
                    convertedFileName = convertedFileDTO.Filename;
                    convertedFileId = convertedFileDTO.QFileServerId;
                }
                catch (Exception ex)
                {
                    successfull = false;
                    errorMessage = ex.Message;
                }
                finally
                {
                    endDate = DateTime.Now;
                }

                await processItemsRepository.UpdateEndInfo(processItemId, endDate, convertedFileId, convertedFileName, successfull, errorMessage);
            }
        }


        private async Task<FFMpegProcess> GetProcessAndItems(int processId, 
            IFFMpegProcessRepository processRepository,
            IFFMpegProcessFeaturesRepository processFeaturesRepository,
            IMapper mapper)
        {
            var eProcess = await processRepository.GetWithItemsAsync(processId);
            var eProcessFeatures = await processFeaturesRepository.GetAsync(processId);
            var ret = mapper.Map<FFMpegProcess>((eProcess, eProcessFeatures));
            return ret;
        }
    }
}
