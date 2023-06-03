using AutoMapper;
using AutoMapper.QueryableExtensions;
using FFMpegUI.Models;
using FFMpegUI.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace FFMpegUI.Persistence.Repositories
{
    public class SQLFFMpegProcessRepository : SQLFFMpegRepository<FFMpegPersistedProcess>, IFFMpegProcessRepository
    {
        private readonly FFMpegDbContext dbContext;
        private readonly IMapper mapper;

        public SQLFFMpegProcessRepository(FFMpegDbContext dbContext,
            IMapper mapper) : base(dbContext)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        async Task<IPagedList<FFMpegProcessSummary>> IFFMpegProcessRepository.GetAllSummaryAsync(int pageNumber, int pageSize)
        {
            var skip = pageSize * (pageNumber - 1);

            var result = dbContext.Processes
                .OrderBy(a => EF.Property<int>(a, "Id"))
                .AsNoTracking()
                .ProjectTo<FFMpegProcessSummary>(mapper.ConfigurationProvider)
                .ToPagedList(pageNumber, pageSize);

            return await Task.FromResult(result);
        }
        
        async Task<FFMpegPersistedProcess> IFFMpegProcessRepository.GetWithItemsAsync(int processId)
        {
            var ret = await dbContext.Processes
                .Include(p => p.Items)
                .Where(p => p.Id == processId)
                .AsNoTracking()
                .SingleAsync();

            return ret;
        }
    }
}
