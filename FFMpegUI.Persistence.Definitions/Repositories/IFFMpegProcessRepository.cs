using FFMpegUI.Models;
using FFMpegUI.Persistence.Definitions.Repositories;
using FFMpegUI.Persistence.Entities;
using PagedList.Core;

namespace FFMpegUI.Persistence.Repositories
{
    public interface IFFMpegProcessRepository : IFFMpegRepository
    {
        Task<FFMpegPersistedProcess> CreateAsync(FFMpegPersistedProcess createdProcess);
        Task<FFMpegPersistedProcess> GetAsync(int processId);
        Task<IPagedList<FFMpegPersistedProcess>> GetAllAsync(int pageNumber, int pageSize);
        Task<IPagedList<FFMpegProcessSummary>> GetAllSummaryAsync(int pageNumber, int pageSize);
        Task<FFMpegPersistedProcess> UpdateAsync(FFMpegPersistedProcess updatedProcess);
        Task DeleteAsync(int processId);
        Task<FFMpegPersistedProcess> GetWithItemsAsync(int processId);
    }
}
