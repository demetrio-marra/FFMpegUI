using System.Collections.Concurrent;

namespace FFMpegUI.Infrastructure.Support
{
    public class ProcessItemBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<ConvertProcessItemTaskRunnerItem> _queue;
        private readonly SemaphoreSlim _signal;

        public ProcessItemBackgroundTaskQueue()
        {
            _queue = new ConcurrentQueue<ConvertProcessItemTaskRunnerItem>();
            _signal = new SemaphoreSlim(0);
        }

        public void Enqueue(ConvertProcessItemTaskRunnerItem processId)
        {
            _queue.Enqueue(processId);
            _signal.Release();
        }

        public async Task<ConvertProcessItemTaskRunnerItem> DequeueAsync()
        {
            await _signal.WaitAsync();
            _queue.TryDequeue(out var processId);

            return processId;
        }
    }
}
