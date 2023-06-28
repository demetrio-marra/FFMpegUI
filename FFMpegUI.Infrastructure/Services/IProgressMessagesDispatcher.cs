using FFMpegUI.Messages;

namespace FFMpegUI.Services.Middlewares
{
    public interface IProgressMessagesDispatcher
    {
        Task DispatchProcessItemProgress(FFMpegProcessItemMessage message);
    }
}
