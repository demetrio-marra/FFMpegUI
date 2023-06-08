using FFMpegUI.Services;
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
                var managementService = scope.ServiceProvider.GetRequiredService<IFFMpegManagementService>();
                await managementService.PerformProcessConversion(processId);
            }
        }
    }
}
