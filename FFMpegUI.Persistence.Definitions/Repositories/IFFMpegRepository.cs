namespace FFMpegUI.Persistence.Definitions.Repositories
{
    public interface IFFMpegRepository
    {
        Task<string> BeginTransactionAsync();
        Task ConfirmTransactionAsync(string transactionId);
        Task RejectTransactionAsync(string transactionId);
    }
}
