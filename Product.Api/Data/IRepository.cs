using System.Linq.Expressions;

namespace Product.Api.Data
{
    public interface IRepository<T>
        where T : class
    {
        Task<T?> GetAsync<TKey>(TKey id);

        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        Task AddAsync(T entity);

        Task UpdateAsync(T entity);

        void Delete(T entity);
    }
}
