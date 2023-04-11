using System.Collections.ObjectModel;

namespace Product.Api.Entities
{
    public class Manufacturer : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public ReadOnlyCollection<(int Count, Entities.Product Product)>? Products { get; set; }
    }
}
