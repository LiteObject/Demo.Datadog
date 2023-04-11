namespace Product.Api.Services
{
    public static class StaticProductService
    {
        private static readonly List<Entities.Product> Products = new()
        {
            new Entities.Product{ Id = 1, Name = "P1", Description = "Product One", UnitPrice = 1 },
            new Entities.Product{ Id = 2, Name = "P2", Description = "Product Two", UnitPrice = 2 }
        };

        public static IReadOnlyCollection<Entities.Product> Find()
        {
            return Products;
        }

        public static Entities.Product? GetById(int id)
        {
            return Products.FirstOrDefault(u => u.Id == id);
        }

        public static void Add(Entities.Product newProduct)
        {
            ArgumentNullException.ThrowIfNull(newProduct, nameof(newProduct));

            int currentMaxId = Products.OrderByDescending(u => u.Id)?.FirstOrDefault()?.Id ?? 0;
            newProduct.Id = currentMaxId + 1;

            Products.Add(newProduct);
        }

        public static void Delete(int id)
        {
            Entities.Product? Product = GetById(id);

            if (Product is not null)
            {
                _ = Products.Remove(Product);
            }
        }

        public static void Update(Entities.Product updatedProduct)
        {
            int index = Products.FindIndex(u => u.Id == updatedProduct.Id);

            if (index != -1)
            {
                Products[index] = updatedProduct;
            }
        }
    }
}
