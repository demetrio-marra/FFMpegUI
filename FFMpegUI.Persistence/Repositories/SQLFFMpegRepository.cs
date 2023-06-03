using FFMpegUI.Persistence.Definitions.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PagedList.Core;

namespace FFMpegUI.Persistence.Repositories
{
    public class SQLFFMpegRepository<TEntity> : IFFMpegRepository where TEntity : class
    {
        private readonly IDictionary<string, IDbContextTransaction> transactions;
        private readonly FFMpegDbContext dbContext;

        public SQLFFMpegRepository(FFMpegDbContext dbContext)
        {
            transactions = new Dictionary<string, IDbContextTransaction>();
            this.dbContext = dbContext;
        }

        async Task IFFMpegRepository.ConfirmTransactionAsync(string transactionId)
        {
            if (!transactions.TryGetValue(transactionId, out var tran))
            {
                throw new Exception($"Invalid transaction id: {transactionId}");
            }

            try
            {
                await tran.CommitAsync();
                await tran.DisposeAsync();
            }
            finally
            {
                transactions.Remove(transactionId);
            }
        }

        async Task<string> IFFMpegRepository.BeginTransactionAsync()
        {
            var tranid = Guid.NewGuid().ToString();
            var tran = await dbContext.Database.BeginTransactionAsync();
            transactions.Add(tranid, tran);
            return tranid;
        }

        async Task IFFMpegRepository.RejectTransactionAsync(string transactionId)
        {
            if (!transactions.TryGetValue(transactionId, out var tran))
            {
                throw new Exception($"Invalid transaction id: {transactionId}");
            }

            try
            {
                await tran.RollbackAsync();
                await tran.DisposeAsync();
            }
            finally
            {
                transactions.Remove(transactionId);
            }
        }


        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            dbContext.Entry(entity).State = EntityState.Added;
            await dbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await dbContext.FindAsync<TEntity>(id);
            if (entity == null)
            {
                return;
            }

            dbContext.Remove(entity);
            await dbContext.SaveChangesAsync();
        }

        public virtual async Task<IPagedList<TEntity>> GetAllAsync(int pageNumber, int pageSize)
        {
            var skip = pageSize * (pageNumber - 1);

            var result = dbContext.Set<TEntity>()
                .OrderBy(a => EF.Property<int>(a, "Id"))
                .AsNoTracking()
                .ToPagedList(pageNumber, pageSize);

            return await Task.FromResult(result);
        }

        public virtual async Task<TEntity> GetAsync(int id)
        {
            return await dbContext.Set<TEntity>()
                .AsNoTracking()
                .SingleAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();

            return entity;
        }
    }
}
