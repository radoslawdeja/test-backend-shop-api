using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Test.Shop.Core.Repositories.Abstractions;

namespace Test.Shop.Infrastructure.DAL.Repositories
{
    /// <summary>
    /// The MappedRepository class inherits from the base Repository<TEntity>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="context"></param>
    /// <param name="mapper"></param>
    public class MappedRepository<TEntity>(ShopDbContext context, IMapper mapper) : Repository<TEntity>(context), IMappedRepository<TEntity>
        where TEntity : class
    {
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Get full Dto list
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        public async Task<IEnumerable<TDto>> GetAllAsync<TDto>()
           => await _mapper.ProjectTo<TDto>(dbSet.AsQueryable()).ToListAsync();

        /// <summary>
        /// Get Dto list filtered by expression
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TDto>> GetByExpressionAsync<TDto>(Expression<Func<TEntity, bool>> expression)
        {
            return await _mapper.ProjectTo<TDto>(dbSet.Where(expression)).ToListAsync();
        }
    }
}
