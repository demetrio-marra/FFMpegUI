using FFMpegUI.Models;
using FFMpegUI.Persistence.Entities;
using FFMpegUI.Resilience;
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


        async Task IFFMpegProcessItemsRepository.UpdateProgressInfo(FFMpegUpdateProcessItemCommand command)
        {
            await sqlPolicy.ExecuteAsync(async () =>
            {
                var item = await dbContext.ProcessItems.FindAsync(command.ProcessItemId);

                if (command.Successfull.HasValue)
                {
                    item.Successfull = command.Successfull;
                }

                if (command.StartDate.HasValue)
                {
                    item.StartDate = command.StartDate;
                }

                if (command.EndDate.HasValue)
                {
                    item.EndDate = command.EndDate;
                }

                if (command.ConvertedFileId.HasValue)
                {
                    item.ConvertedFileId = command.ConvertedFileId;
                }

                if (!string.IsNullOrEmpty(command.ConvertedFileName))
                {
                    item.ConvertedFileName = command.ConvertedFileName;
                }

                if (command.ConvertedFileSize.HasValue)
                {
                    item.ConvertedFileSize = command.ConvertedFileSize;
                }

                if (!string.IsNullOrEmpty(command.StatusMessage))
                {
                    item.StatusMessage = command.StatusMessage;
                }

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
