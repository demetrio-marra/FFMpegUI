using AutoMapper;
using AutoMapper.QueryableExtensions;
using FFMpegUI.Models;
using FFMpegUI.Persistence.Entities;
using FFMpegUI.Resilience;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using Polly;

namespace FFMpegUI.Persistence.Repositories
{
    public class SQLFFMpegProcessRepository : SQLFFMpegRepository<FFMpegPersistedProcess>, IFFMpegProcessRepository
    {
        private readonly FFMpegDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IAsyncPolicy sqlPolicy;

        public SQLFFMpegProcessRepository(FFMpegDbContext dbContext,
            IMapper mapper, IResilientPoliciesLocator policiesLocator) : base(dbContext, policiesLocator)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            sqlPolicy = policiesLocator.GetPolicy(ResilientPolicyType.SqlDatabase);
        }

        async Task<IPagedList<FFMpegProcessSummary>> IFFMpegProcessRepository.GetAllSummaryAsync(int pageNumber, int pageSize)
        {
            var skip = pageSize * (pageNumber - 1);

            var ret = await sqlPolicy.ExecuteAsync(async () =>
            {
                var res = dbContext.Processes
                .OrderBy(a => EF.Property<int>(a, "Id"))
                .AsNoTracking()
                .ProjectTo<FFMpegProcessSummary>(mapper.ConfigurationProvider)
                .ToPagedList(pageNumber, pageSize);

                return await Task.FromResult(res);
            });

            return ret;
        }
        
        async Task<FFMpegPersistedProcess> IFFMpegProcessRepository.GetWithItemsAsync(int processId)
        {
            var ret = await sqlPolicy.ExecuteAsync(async () =>
            {
                var res = await dbContext.Processes
                .Include(p => p.Items)
                .Where(p => p.Id == processId)
                .AsNoTracking()
                .SingleAsync();

                return res;
            });

            return ret;
        }

        async Task IFFMpegProcessRepository.UpdateConversionCompletedData(int processId, long convertedFilesTotalSize, DateTime? processEndDate)
        {
            await sqlPolicy.ExecuteAsync(async () =>
            {
                var process = await dbContext.Processes.FindAsync(processId);
                process.ConvertedFilesTotalSize = convertedFilesTotalSize;
                process.EndDate = processEndDate;
                await dbContext.SaveChangesAsync();
            });
        }
    }
}
