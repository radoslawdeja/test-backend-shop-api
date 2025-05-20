using System.Linq.Expressions;

namespace Test.Shop.Core.Repositories.Abstractions
{
    public interface IMappedRepository<T> : IRepository<T>
        where T : class
    {
        /// <summary>
        /// Get full Dto list
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        Task<IEnumerable<TDto>> GetAllAsync<TDto>();

        /// <summary>
        /// Get Dto list filtered by expression
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<IEnumerable<TDto>> GetByExpressionAsync<TDto>(Expression<Func<T, bool>> expression);
    }
}
