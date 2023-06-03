using FFMpegUI.Persistence.Definitions.Repositories;
using FFMpegUI.Persistence.Entities;
using PagedList.Core;

namespace FFMpegUI.Persistence.Repositories
{
    public interface IFFMpegProcessFeaturesRepository : IFFMpegRepository
    {
        Task<FFMpegPersistedProcessFeatures> CreateAsync(FFMpegPersistedProcessFeatures createdProcessFeature);
        Task<FFMpegPersistedProcessFeatures> GetAsync(int processFeatureId);
        Task<IPagedList<FFMpegPersistedProcessFeatures>> GetAllAsync(int pageNumber, int pageSize);
        Task<FFMpegPersistedProcessFeatures> UpdateAsync(FFMpegPersistedProcessFeatures updatedProcessFeature);
        Task DeleteAsync(int processFeatureId);
    }
}
