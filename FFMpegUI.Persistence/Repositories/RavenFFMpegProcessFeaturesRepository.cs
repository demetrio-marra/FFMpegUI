using AutoMapper;
using FFMpegUI.Persistence.Definitions.Repositories;
using FFMpegUI.Persistence.Entities;
using PagedList.Core;

namespace FFMpegUI.Persistence.Repositories
{
    public class RavenFFMpegProcessFeaturesRepository : IFFMpegProcessFeaturesRepository
    {
        private readonly IMapper mapper;

        public RavenFFMpegProcessFeaturesRepository(
            IMapper mapper
            )
        {
            this.mapper = mapper;
        }

        async Task IFFMpegRepository.ConfirmTransactionAsync(string transactionId)
        {
            await Task.CompletedTask; // raven no transaction
        }

        async Task<string> IFFMpegRepository.BeginTransactionAsync()
        {
            return await Task.FromResult(string.Empty); // raven no transaction
        }

        async Task<FFMpegPersistedProcessFeatures> IFFMpegProcessFeaturesRepository.CreateAsync(FFMpegPersistedProcessFeatures createdProcessFeature)
        {
            return await Task.FromResult(createdProcessFeature);
        }

        async Task IFFMpegProcessFeaturesRepository.DeleteAsync(int processFeatureId)
        {
            await Task.CompletedTask;
        }

        async Task<IPagedList<FFMpegPersistedProcessFeatures>> IFFMpegProcessFeaturesRepository.GetAllAsync(int pageNumber, int pageSize)
        {
            return new PagedList<FFMpegPersistedProcessFeatures>(Enumerable.Empty<FFMpegPersistedProcessFeatures>(), 1, 10);
        }

        async Task<FFMpegPersistedProcessFeatures> IFFMpegProcessFeaturesRepository.GetAsync(int processFeatureId)
        {
            return await Task.FromResult(new  FFMpegPersistedProcessFeatures());
        }

        async Task IFFMpegRepository.RejectTransactionAsync(string transactionId)
        {
            await Task.CompletedTask; // raven no transaction
        }

        async Task<FFMpegPersistedProcessFeatures> IFFMpegProcessFeaturesRepository.UpdateAsync(FFMpegPersistedProcessFeatures updatedProcessFeature)
        {
            return await Task.FromResult(updatedProcessFeature);
        }
    }
}
