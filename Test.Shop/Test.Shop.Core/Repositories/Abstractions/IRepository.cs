using System.Linq.Expressions;

namespace Test.Shop.Core.Repositories.Abstractions
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetQuery();

        Task AddRangeAsync(IEnumerable<T> datas, bool save = true, CancellationToken cancellationToken = default);

        Task WriteRangeAsync(IEnumerable<T> datas, bool save = true, CancellationToken cancellationToken = default);

        Task Truncate(bool save = true, CancellationToken cancellationToken = default);

        Task DeleteByExpression(Expression<Func<T, bool>> expression, bool save = true, CancellationToken cancellationToken = default);

        Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default);

        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellation = default);

        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetByExpression(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        Task UpdateRangeAsync(IEnumerable<T> datas, bool save = true, CancellationToken cancellationToken = default);

        Task UpdateAsync(T item, bool save = true, CancellationToken cancellationToken = default);

        Task AddAsync(T item, bool save = true, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);

        ValueTask BulkInsertOrUpdateAsync(IAsyncEnumerable<T> datas, int buffer = 1000, CancellationToken cancellationToken = default);

        ValueTask BulkInsertAsync(IAsyncEnumerable<T> datas, int buffer = 1000, CancellationToken cancellationToken = default);

        ValueTask BulkInsertAsync(IEnumerable<T> datas);

        Task InsertOrUpdateRangeAsync(IEnumerable<T> datas);

        Task<IEnumerable<int>> GetIdsAsync(Expression<Func<T, bool>> expression, Expression<Func<T, int>> selector);

        Task<IEnumerable<TSelect>> GetByConditionAsync<TSelect>(Expression<Func<T, bool>> predicate, Expression<Func<T, TSelect>> selector, bool distinct = false);

        Task<int> CountGroupsAsync<TKey>(Expression<Func<T, TKey>> groupBySelector, Expression<Func<T, bool>>? predicate = null, Expression<Func<IGrouping<TKey, T>, bool>>? groupFilter = null);
    }
}
