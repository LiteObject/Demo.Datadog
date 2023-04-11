using Product.Api.Data;
using System.Linq.Expressions;

namespace Product.Api.Services
{
    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IRepository<Entities.Product> _repository;

        public ProductService(ILogger<ProductService> logger, IRepository<Entities.Product> repository)
        {
            _logger = logger;
            _repository = repository;

            _logger.LogInf($"Instantiated {nameof(ProductService)} class.");
        }

        public async Task<List<Entities.Product>> FindAsync(Expression<Func<Entities.Product, bool>> predicate)
        {
            // ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            _logger.LogDbg($"Invoked {nameof(FindAsync)}. Arg: {predicate?.Body}");
            List<Entities.Product> products = await _repository.FindAsync(predicate);
            return products;
        }

        public async Task<Entities.Product?> GetByIdAsync(int id)
        {
            _logger.LogDbg($"Invoked {nameof(GetByIdAsync)}. Arg: {id}");

            Entities.Product? product = await _repository.GetAsync(id);
            return product;
        }
    }
}
