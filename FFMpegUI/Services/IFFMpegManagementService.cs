using FFMpegUI.Models;
using PagedList.Core;

namespace FFMpegUI.Services
{
    public interface IFFMpegManagementService
    {
        Task<IPagedList<FFMpegProcessSummary>> GetProcessesSummary(int pageNumber, int pageSize);

        Task CreateProcess(FFMpegCreateProcessCommand command);

        Task<FFMpegProcess> GetProcessDetails(int processId);

        Task<FFMpegFileDownloadDTO> GetFileForDownload(long fileId);
    }
}
