using FFMpegUI.Persistence.Definitions.Repositories;
using FFMpegUI.Resilience;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PagedList.Core;
using Polly;

namespace FFMpegUI.Persistence.Repositories
{
    public class SQLFFMpegRepository<TEntity> : IFFMpegRepository where TEntity : class
    {
        private readonly IDictionary<string, IDbContextTransaction> transactions;
        private readonly FFMpegDbContext dbContext;
        private readonly IAsyncPolicy sqlPolicy;

        public SQLFFMpegRepository(FFMpegDbContext dbContext, IResilientPoliciesLocator policiesLocator)
        {
            transactions = new Dictionary<string, IDbContextTransaction>();
            this.dbContext = dbContext;
            sqlPolicy = policiesLocator.GetPolicy(ResilientPolicyType.SqlDatabase);
        }

        async Task IFFMpegRepository.ConfirmTransactionAsync(string transactionId)
        {
            if (!transactions.TryGetValue(transactionId, out var tran))
            {
                throw new Exception($"Invalid transaction id: {transactionId}");
            }

            try
            {
                await sqlPolicy.ExecuteAsync(async () =>
                {
                    await tran.CommitAsync();
                    await tran.DisposeAsync();
                });
            }
            finally
            {
                transactions.Remove(transactionId);
            }
        }

        async Task<string> IFFMpegRepository.BeginTransactionAsync()
        {
            var tranid = Guid.NewGuid().ToString();

            await sqlPolicy.ExecuteAsync(async () =>
            {
                var tran = await dbContext.Database.BeginTransactionAsync();
                transactions.Add(tranid, tran);
            });

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
                await sqlPolicy.ExecuteAsync(async () =>
                {
                    await tran.RollbackAsync();
                    await tran.DisposeAsync();
                });
            }
            finally
            {
                transactions.Remove(transactionId);
            }
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
