using Microsoft.EntityFrameworkCore.Storage;

namespace FFMpegUI.Persistence
{
    /// <summary>
    /// Singleton
    /// </summary>
    public class FFMpegUITransactionsTracker
    {
        private readonly SemaphoreSlim semaphore = new(1, 1); 
        private readonly IDictionary<string, IDbContextTransaction> transactions = new Dictionary<string, IDbContextTransaction>();


        public async Task<string> AddTransaction(IDbContextTransaction transaction)
        {
            await semaphore.WaitAsync();  // Acquire the semaphore
            try
            {
                var guid = Guid.NewGuid().ToString();
                transactions.Add(guid, transaction);

                return guid;
            }
            finally
            {
                semaphore.Release();  // Release the semaphore
            }
        }


        public async Task<IDbContextTransaction?> GetAndRemoveTransaction(string id)
        {
            await semaphore.WaitAsync();  // Acquire the semaphore
            try
            {
                transactions.TryGetValue(id, out var transaction);

                return transaction;
            }
            finally
            {
                semaphore.Release();  // Release the semaphore
            }
        }
    }
}
