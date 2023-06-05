using FFMpegUI.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

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

        async Task IFFMpegProcessItemsRepository.UpdateEndInfo(int processItemId, DateTime? endDate, long? convertedFileId, string? convertedFileName, bool? success, string? errorMessage)
        {
            var item = await dbContext.ProcessItems.FindAsync(processItemId);
            item.EndDate = endDate;
            item.ConvertedFileId = convertedFileId;
            item.ConvertedFileName = convertedFileName;
            item.Successfull = success;
            item.ErrorMessage = errorMessage;
            await dbContext.SaveChangesAsync();
        }

        async Task IFFMpegProcessItemsRepository.UpdateStartInfo(int processItemId, DateTime startDate)
        {
            var item = await dbContext.ProcessItems.FindAsync(processItemId);
            item.StartDate = startDate;
            await dbContext.SaveChangesAsync();
        }
    }
}
