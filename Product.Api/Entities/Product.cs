using System.ComponentModel.DataAnnotations;

namespace Product.Api.Entities
{
    public class Product : BaseEntity
    {
        [Required]
        [StringLength(25)]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Range(0, 99.99)]
        public decimal UnitPrice { get; set; }
        public bool IsAvailable { get; set; }
    }
}