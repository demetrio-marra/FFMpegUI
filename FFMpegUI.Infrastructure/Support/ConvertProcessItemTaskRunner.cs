using FFMpegUI.Services;
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


        private async Task ConvertJob(ConvertProcessItemTaskRunnerItem processItemId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var managementService = scope.ServiceProvider.GetRequiredService<IFFMpegConvertingService>();
                
                await managementService.Convert(processItemId.QFileServerFileId, processItemId.ConvertParameters);
            }
        }
    }
}
