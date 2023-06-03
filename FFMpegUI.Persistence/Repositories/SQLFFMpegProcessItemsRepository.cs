using FFMpegUI.Persistence.Entities;

namespace FFMpegUI.Persistence.Repositories
{
    public class SQLFFMpegProcessItemsRepository : SQLFFMpegRepository<FFMpegPersistedProcessItem>, IFFMpegProcessItemsRepository
    {
        private readonly FFMpegDbContext dbContext;

        public SQLFFMpegProcessItemsRepository(FFMpegDbContext dbContext) : base(dbContext) 
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<FFMpegPersistedProcessItem>> CreateAsync(IEnumerable<FFMpegPersistedProcessItem> createdProcessItems)
        {
            dbContext.ProcessItems.AddRange(createdProcessItems);
            await dbContext.SaveChangesAsync();
            return createdProcessItems;
        }
    }
}
