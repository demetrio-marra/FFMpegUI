using FFMpegUI.Persistence.Entities;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;

namespace FFMpegUI.Persistence.Repositories
{
    public class RavenFFMpegProcessFeaturesRepository : IFFMpegProcessFeaturesRepository
    {
        private readonly IDocumentStore documentStore;

        public RavenFFMpegProcessFeaturesRepository(
            IDocumentStore documentStore
            )
        {
            this.documentStore = documentStore;
        }

        async Task IFFMpegProcessFeaturesRepository.CreateAsync(FFMpegPersistedProcessFeatures createdProcessFeature)
        {
            using (var session = documentStore.OpenSession())
            {
                session.Store(createdProcessFeature);
                session.SaveChanges();
            }

            await Task.CompletedTask;
        }

        async Task IFFMpegProcessFeaturesRepository.DeleteAsync(int processFeatureId)
        {
            using (var session = documentStore.OpenSession())
            {
                var product = session.Query<FFMpegPersistedProcessFeatures>()
                    .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(10)))
                    .Where(p => p.ProcessId == processFeatureId)
                    .Single();
                session.Delete(product);
                session.SaveChanges();
            }
        }

    
        async Task<FFMpegPersistedProcessFeatures> IFFMpegProcessFeaturesRepository.GetAsync(int processFeatureId)
        {
            using var session = documentStore.OpenSession();
            // Use the session to query or store data
            var results = session.Query<FFMpegPersistedProcessFeatures>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(10)))
                .Where(p => p.ProcessId == processFeatureId)
                .Single();

            return await Task.FromResult(results);

        }
    }
}
