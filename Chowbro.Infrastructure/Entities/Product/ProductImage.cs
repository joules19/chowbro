using System.ComponentModel.DataAnnotations;

namespace Chowbro.Infrastructure.Entities.Vendor
{
    public class ProductImage : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();  
        [Required]
        public string ImageUrl { get; set; }
        public string Caption { get; set; }

        // Foreign key
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}