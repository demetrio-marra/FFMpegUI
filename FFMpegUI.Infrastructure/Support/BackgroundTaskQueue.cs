using System.Collections.Concurrent;

namespace FFMpegUI.Infrastructure.Support
{
    public class BackgroundTaskQueue
    {
        private readonly ConcurrentQueue<int> _queue;
        private readonly SemaphoreSlim _signal;

        public BackgroundTaskQueue()
        {
            _queue = new ConcurrentQueue<int>();
            _signal = new SemaphoreSlim(0);
        }

        public void Enqueue(int processId)
        {
            _queue.Enqueue(processId);
            _signal.Release();
        }

        public async Task<int> DequeueAsync()
        {
            await _signal.WaitAsync();
            _queue.TryDequeue(out var processId);

            return processId;
        }
    }
}
