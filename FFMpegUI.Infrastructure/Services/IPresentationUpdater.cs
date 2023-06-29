using FFMpegUI.Models;

namespace FFMpegUI.Infrastructure.Services
{
    public interface IPresentationUpdater
    {
        Task UpdateProcessItem(FFMpegProcessItemStatusNotification message);
        Task UpdateProcess(FFMpegProcessStatusNotification message);
    }
}
