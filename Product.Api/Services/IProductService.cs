using System.Linq.Expressions;

namespace Product.Api.Services
{
    public interface IProductService
    {
        public Task<Entities.Product?> GetByIdAsync(int id);

        public Task<List<Entities.Product>> FindAsync(Expression<Func<Entities.Product, bool>> predicate);
    }
}
