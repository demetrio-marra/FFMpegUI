using FFMpegUI.Persistence.Definitions.Repositories;
using FFMpegUI.Resilience;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using Polly;

namespace FFMpegUI.Persistence.Repositories
{
    public class SQLFFMpegRepository<TEntity> : IFFMpegRepository where TEntity : class
    {
        private readonly FFMpegDbContext dbContext;
        private readonly IAsyncPolicy sqlPolicy;
        private readonly FFMpegUITransactionsTracker transactionsTracker;


        public SQLFFMpegRepository(FFMpegDbContext dbContext, IResilientPoliciesLocator policiesLocator, FFMpegUITransactionsTracker transactionsTracker)
        {
            this.dbContext = dbContext;
            sqlPolicy = policiesLocator.GetPolicy(ResilientPolicyType.SqlDatabase);
            this.transactionsTracker = transactionsTracker;
        }


        async Task IFFMpegRepository.ConfirmTransactionAsync(string transactionId)
        {
            var tran = await transactionsTracker.GetAndRemoveTransaction(transactionId);
            if (tran == null)
            {
                throw new Exception($"Invalid transaction id: {transactionId}");
            }

            await sqlPolicy.ExecuteAsync(async () =>
            {
                await tran.CommitAsync();
                await tran.DisposeAsync();
            });
        }


        async Task<string> IFFMpegRepository.BeginTransactionAsync()
        {
            var tran = await sqlPolicy.ExecuteAsync(async () =>
            {
                var tran = await dbContext.Database.BeginTransactionAsync();
                return tran;
            });

            var tranid = await transactionsTracker.AddTransaction(tran);

            return tranid;
        }


        async Task IFFMpegRepository.RejectTransactionAsync(string transactionId)
        {
            var tran = await transactionsTracker.GetAndRemoveTransaction(transactionId);
            if (tran == null)
            {
                throw new Exception($"Invalid transaction id: {transactionId}");
            }

            await sqlPolicy.ExecuteAsync(async () =>
            {
                await tran.RollbackAsync();
                await tran.DisposeAsync();
            });
        }


        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            await sqlPolicy.ExecuteAsync(async () =>
            {
                dbContext.Entry(entity).State = EntityState.Added;
                await dbContext.SaveChangesAsync();
            });

            return entity;
        }


        public virtual async Task DeleteAsync(int id)
        {
            await sqlPolicy.ExecuteAsync(async () =>
            {
                var entity = await dbContext.FindAsync<TEntity>(id);
                if (entity == null)
                {
                    return;
                }

                dbContext.Remove(entity);
                await dbContext.SaveChangesAsync();
            });
        }


        public virtual async Task<IPagedList<TEntity>> GetAllAsync(int pageNumber, int pageSize)
        {
            var skip = pageSize * (pageNumber - 1);

            var ret = await sqlPolicy.ExecuteAsync(async () =>
            {
                var res = dbContext.Set<TEntity>()
                    .OrderBy(a => EF.Property<int>(a, "Id"))
                    .AsNoTracking()
                    .ToPagedList(pageNumber, pageSize);

                return await Task.FromResult(res);
            });

            return await Task.FromResult(ret);
        }


        public virtual async Task<TEntity> GetAsync(int id)
        {
            var ret = await sqlPolicy.ExecuteAsync(async () =>
            {
                var res = await dbContext.Set<TEntity>()
                .AsNoTracking()
                .SingleAsync(e => EF.Property<int>(e, "Id") == id);

                return res;
            });

            return ret;
        }


        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            await sqlPolicy.ExecuteAsync(async () =>
            {
                dbContext.Entry(entity).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();
            });

            return entity;
        }
    }
}
