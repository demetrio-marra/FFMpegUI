using FFMpegUI.Messages;
using FFMpegUI.Services;
using FFMpegUI.Services.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FFMpegUI.Infrastructure.Support
{
    public class ConvertProcessItemTaskRunner : BackgroundService
    {
        private readonly ProcessItemBackgroundTaskQueue taskQueue;
        private readonly IServiceProvider _serviceProvider;


        public ConvertProcessItemTaskRunner(ProcessItemBackgroundTaskQueue taskQueue,
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
                var processItemId = await taskQueue.DequeueAsync();
                await ConvertJob(processItemId);
            }
        }


        private async Task ConvertJob(ConvertProcessItemTaskRunnerItem processItem)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var convertingService = scope.ServiceProvider.GetRequiredService<IFFMpegConvertingService>();
                var progressMessagedDispatcher = scope.ServiceProvider.GetRequiredService<IProgressMessagesDispatcher>();

                var startProgressMessage = new FFMpegProcessItemMessage
                {
                    ProcessItemId = processItem.ProcessItemId,
                    StartDate = DateTime.Now,
                    ProgressMessage = "starting..."
                };

                await progressMessagedDispatcher.DispatchProcessItemProgress(startProgressMessage);

                var progressMessage = new FFMpegProcessItemMessage
                {
                    ProcessItemId = processItem.ProcessItemId
                };

                try
                {
                    var convertedFileDTO = await convertingService.ConvertEmittingMessages(processItem.QFileServerFileId, processItem.ProcessItemId, processItem.ConvertParameters);

                    progressMessage.ConvertedFileId = convertedFileDTO.QFileServerId;
                    progressMessage.ProgressMessage = "ok";
                    progressMessage.ConvertedFileName = convertedFileDTO.Filename;
                    progressMessage.ConvertedFileSize = convertedFileDTO.Filesize;
                    progressMessage.Successfull = true;
                } 
                catch (Exception ex)
                {
                    progressMessage.ProgressMessage = "ERROR: " + ex.Message;
                }
                finally
                {
                    progressMessage.EndDate = DateTime.Now;
                }

                
                await progressMessagedDispatcher.DispatchProcessItemProgress(progressMessage);
            }
        }
    }
}
