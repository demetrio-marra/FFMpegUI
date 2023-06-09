using FFMpegUI.Persistence.Entities;
using FFMpegUI.Resilience;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace FFMpegUI.Persistence.Repositories
{
    public class SQLFFMpegProcessItemsRepository : SQLFFMpegRepository<FFMpegPersistedProcessItem>, IFFMpegProcessItemsRepository
    {
        private readonly FFMpegDbContext dbContext;
        private readonly IAsyncPolicy sqlPolicy;

        public SQLFFMpegProcessItemsRepository(FFMpegDbContext dbContext, IResilientPoliciesLocator policiesLocator) : base(dbContext, policiesLocator) 
        {
            this.dbContext = dbContext;
            sqlPolicy = policiesLocator.GetPolicy(ResilientPolicyType.SqlDatabase);
        }

        public async Task<IEnumerable<FFMpegPersistedProcessItem>> CreateAsync(IEnumerable<FFMpegPersistedProcessItem> createdProcessItems)
        {
            await sqlPolicy.ExecuteAsync(async () =>
            {
                dbContext.ProcessItems.AddRange(createdProcessItems);
                await dbContext.SaveChangesAsync();
            });

            return createdProcessItems;
        }

        async Task IFFMpegProcessItemsRepository.UpdateEndInfo(int processItemId, DateTime? endDate, long? convertedFileId, string? convertedFileName, long? convertedFileSize, bool? success, string? errorMessage)
        {
            await sqlPolicy.ExecuteAsync(async () =>
            {
                var item = await dbContext.ProcessItems.FindAsync(processItemId);
                item.EndDate = endDate;
                item.ConvertedFileId = convertedFileId;
                item.ConvertedFileName = convertedFileName;
                item.Successfull = success;
                item.ErrorMessage = errorMessage;
                item.ConvertedFileSize = convertedFileSize;
                await dbContext.SaveChangesAsync();
            });
        }

        async Task IFFMpegProcessItemsRepository.UpdateStartInfo(int processItemId, DateTime startDate)
        {
            await sqlPolicy.ExecuteAsync(async () =>
            {
                var item = await dbContext.ProcessItems.FindAsync(processItemId);
                item.StartDate = startDate;
                await dbContext.SaveChangesAsync();
            });
        }
    }
}
