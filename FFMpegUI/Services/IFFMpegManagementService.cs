using FFMpegUI.Messages;
using FFMpegUI.Models;
using PagedList.Core;

namespace FFMpegUI.Services
{
    public interface IFFMpegManagementService
    {
        Task<IPagedList<FFMpegProcessSummary>> GetProcessesSummary(int pageNumber, int pageSize);

        Task<FFMpegProcess> CreateProcess(FFMpegCreateProcessCommand command);
        Task<FFMpegProcess> CreateProcess(FFMpegCreateProcessAltCommand command);

        Task<FFMpegProcess> GetProcessDetails(int processId);

        Task<FFMpegFileDownloadDTO> GetFileForDownload(long fileId);

        Task PerformProcessConversion(int processId);

        Task ElaborateProcessItemProgressMessage(FFMpegProcessItemMessage message);
    }
}
