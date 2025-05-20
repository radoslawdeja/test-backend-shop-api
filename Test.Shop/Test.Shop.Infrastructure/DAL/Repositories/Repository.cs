using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System.Buffers;
using System.Linq.Expressions;
using Test.Shop.Core.Repositories.Abstractions;

namespace Test.Shop.Infrastructure.DAL.Repositories
{
    public class Repository<T>(ShopDbContext dbContext) : IRepository<T> where T : class
    {
        #region Fields

        protected readonly ShopDbContext _dbContext = dbContext;

        protected readonly DbSet<T> dbSet = dbContext.Set<T>();

        #endregion Fields

        #region IQueryable

        public IQueryable<T> GetQuery() => dbSet;

        #endregion

        #region Methods

        public async Task AddRangeAsync(IEnumerable<T> datas, bool save = true, CancellationToken cancellationToken = default)
        {
            await dbSet.AddRangeAsync(datas, cancellationToken);
            await SaveChangesAsyncConditional(save, cancellationToken);
        }

        public virtual async Task Truncate(bool save = true, CancellationToken cancellationToken = default)
        {
            await DeleteByExpression(x => true, save, cancellationToken);
        }

        public virtual async Task DeleteByExpression(Expression<Func<T, bool>> expression, bool save = true, CancellationToken cancellationToken = default)
        {
            await dbSet.AsNoTracking().Where(expression).ExecuteDeleteAsync(cancellationToken);
            await SaveChangesAsyncConditional(save, cancellationToken);
        }

        public virtual async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(action);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                await action(cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellation = default)
            => await dbSet.FirstOrDefaultAsync(predicate, cancellation);

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
            => await dbSet.ToListAsync(cancellationToken);

        public async Task<IEnumerable<T>> GetByExpression(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
            => await dbSet.Where(expression).ToListAsync(cancellationToken);

        public async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (page < 1 || pageSize < 1)
            {
                throw new ArgumentException($"Cannot get data with page={page} or pageSize={pageSize}");
            }

            var itemsToSkip = (page - 1) * pageSize;

            return await dbSet
                .Skip(itemsToSkip)
                .Take(pageSize).ToListAsync(cancellationToken);
        }

        public async Task UpdateRangeAsync(IEnumerable<T> datas, bool save = true, CancellationToken cancellationToken = default)
        {
            foreach (var data in datas)
            {
                var keyValues = GetKeyValues(data);
                var existingEntity = await dbSet.FindAsync(keyValues);

                if (existingEntity != null)
                {
                    _dbContext.Entry(existingEntity).CurrentValues.SetValues(data);
                    _dbContext.Entry(existingEntity).State = EntityState.Modified;
                }
            }
            await SaveChangesAsyncConditional(save, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await SaveChangesAsyncConditional(true, cancellationToken);
        }

        private async Task SaveChangesAsyncConditional(bool save = true, CancellationToken cancellationToken = default)
        {
            if (save)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        private object[] GetKeyValues(T entity)
        {
            var keyProperties = _dbContext.Model?.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties;
            var keyValues = new List<object>();

            ArgumentNullException.ThrowIfNull(keyProperties);

            foreach (var property in keyProperties)
            {
                var propertyInfo = typeof(T).GetProperty(property.Name);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(entity);
                    if (value != null)
                    {
                        keyValues.Add(value);
                    }
                }
            }

            return [.. keyValues];
        }

        public async Task UpdateAsync(T item, bool save = true, CancellationToken cancellationToken = default)
        {
            await UpdateRangeAsync([item], save, cancellationToken);
        }

        public async Task AddAsync(T item, bool save = true, CancellationToken cancellationToken = default)
        {
            await AddRangeAsync([item], save, cancellationToken);
        }

        public async Task WriteRangeAsync(IEnumerable<T> datas, bool save = true, CancellationToken cancellationToken = default)
        {
            await ExecuteInTransactionAsync(async ct =>
            {
                await DeleteByExpression(x => true, true, ct);
                await AddRangeAsync(datas, true, ct);
            }, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> expression)
            => await dbSet.AnyAsync(expression);

        public virtual async ValueTask BulkInsertOrUpdateAsync(IAsyncEnumerable<T> datas, int buffer = 1000, CancellationToken cancellationToken = default)
        {
            var customMigration = _dbContext.Database.CurrentTransaction == null;

            var transaction = customMigration ? await _dbContext.Database.BeginTransactionAsync() : _dbContext.Database.CurrentTransaction;
            var entitiesToInsert = ArrayPool<T>.Shared.Rent(buffer);
            try
            {
                int count = 0;

                await foreach (var entity in datas.WithCancellation(cancellationToken))
                {
                    entitiesToInsert[count++] = entity;

                    if (count == buffer)
                    {
                        await _dbContext.BulkInsertOrUpdateAsync(entitiesToInsert.Take(count));
                        await _dbContext.BulkSaveChangesAsync();
                        count = 0;
                    }
                }

                if (count > 0)
                {
                    await _dbContext.BulkInsertOrUpdateAsync(entitiesToInsert.Take(count));
                    await _dbContext.BulkSaveChangesAsync();
                }

                if (customMigration && transaction != null) await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (customMigration && transaction != null) await transaction.RollbackAsync(cancellationToken);
                throw;
            }
            finally
            {
                ArrayPool<T>.Shared.Return(entitiesToInsert);
                if (customMigration) transaction?.Dispose();
            }
        }

        public async Task InsertOrUpdateRangeAsync(IEnumerable<T> datas)
        {
            if (datas.Count() > 10)
            {
                await _dbContext.BulkInsertOrUpdateAsync(datas);
                await _dbContext.BulkSaveChangesAsync();
                return;
            }

            foreach (var entity in datas)
            {
                var existingEntity = await _dbContext.Set<T>().FindAsync(GetKeyValues(entity));

                if (existingEntity != null)
                {
                    _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
                }
                else
                {
                    await _dbContext.Set<T>().AddAsync(entity);
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        public virtual async ValueTask BulkInsertAsync(IAsyncEnumerable<T> datas, int buffer = 1000, CancellationToken cancellationToken = default)
        {
            var customMigration = _dbContext.Database.CurrentTransaction == null;

            var transaction = customMigration ? await _dbContext.Database.BeginTransactionAsync() : _dbContext.Database.CurrentTransaction;
            var entitiesToInsert = ArrayPool<T>.Shared.Rent(buffer);
            try
            {
                int count = 0;

                await foreach (var entity in datas.WithCancellation(cancellationToken))
                {
                    entitiesToInsert[count++] = entity;

                    if (count == buffer)
                    {
                        await _dbContext.BulkInsertAsync(entitiesToInsert.Take(count));
                        await _dbContext.BulkSaveChangesAsync();
                        count = 0;
                    }
                }

                if (count > 0)
                {
                    await _dbContext.BulkInsertAsync(entitiesToInsert.Take(count));
                    await _dbContext.BulkSaveChangesAsync();
                }

                if (customMigration && transaction != null) await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (customMigration && transaction != null) await transaction.RollbackAsync(cancellationToken);
                throw;
            }
            finally
            {
                ArrayPool<T>.Shared.Return(entitiesToInsert);
                if (customMigration) transaction?.Dispose();
            }
        }

        public virtual async ValueTask BulkInsertAsync(IEnumerable<T> datas)
        {
            await _dbContext.BulkInsertAsync(datas);
            await _dbContext.BulkSaveChangesAsync();
        }

        public async Task<IEnumerable<int>> GetIdsAsync(Expression<Func<T, bool>> expression, Expression<Func<T, int>> selector)
           => await dbSet.Where(expression).Select(selector).ToListAsync();

        public async Task<IEnumerable<TSelect>> GetByConditionAsync<TSelect>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TSelect>> selector,
            bool distinct = false)
        {
            IQueryable<T> query = dbSet;

            var projected = query.Where(predicate).Select(selector);
            if (distinct)
            {
                projected = projected.Distinct();
            }

            return await projected.ToListAsync();
        }

        public async Task<int> CountGroupsAsync<TKey>(Expression<Func<T, TKey>> groupBySelector, Expression<Func<T, bool>>? predicate = null, Expression<Func<IGrouping<TKey, T>, bool>>? groupFilter = null)
        {
            IQueryable<T> query = dbSet;

            if (predicate is not null)
            {
                query = query.Where(predicate);
            }

            var grouped = query.GroupBy(groupBySelector);

            if (groupFilter is null)
            {
                return await grouped.CountAsync();
            }

            return await grouped.Where(groupFilter).CountAsync();
        }

        #endregion Methods
    }
}
