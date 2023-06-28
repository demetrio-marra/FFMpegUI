using FFMpegUI.Models;
using FFMpegUI.Persistence.Definitions.Repositories;
using FFMpegUI.Persistence.Entities;
using PagedList.Core;

namespace FFMpegUI.Persistence.Repositories
{
    public interface IFFMpegProcessItemsRepository : IFFMpegRepository
    {
        Task<FFMpegPersistedProcessItem> CreateAsync(FFMpegPersistedProcessItem createdProcessItem);
        Task<FFMpegPersistedProcessItem> GetAsync(int processItemId);
        Task<IPagedList<FFMpegPersistedProcessItem>> GetAllAsync(int pageNumber, int pageSize);
        Task UpdateStartInfo(int processItemId, DateTime startDate);
        Task UpdateEndInfo(int processItemId, DateTime? endDate, long? convertedFileId, string? convertedFileName, long? convertedFileSize, bool? success, string? statusMessage);

        Task DeleteAsync(int processItemId);
        Task<IEnumerable<FFMpegPersistedProcessItem>> CreateAsync(IEnumerable<FFMpegPersistedProcessItem> createdProcessItems);

        Task UpdateAsync(FFMpegUpdateProcessItemCommand command);
    }
}
