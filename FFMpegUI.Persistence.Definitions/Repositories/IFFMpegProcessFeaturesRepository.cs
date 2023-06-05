using FFMpegUI.Persistence.Entities;

namespace FFMpegUI.Persistence.Repositories
{
    public interface IFFMpegProcessFeaturesRepository
    {
        Task CreateAsync(FFMpegPersistedProcessFeatures createdProcessFeature);
        Task<FFMpegPersistedProcessFeatures> GetAsync(int processId);
        Task DeleteAsync(int processId);
    }
}
