using FFMpegUI.Messages;

namespace FFMpegUI.Infrastructure.Services
{
    public interface IPresentationUpdater
    {
        Task UpdateProcessItem(FFMpegProcessItemMessage message);
    }
}
