using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chowbro.Infrastructure.Entities.Vendor
{
    public class ProductOptionCategory : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();  
        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        public string? Description { get; set; }
        public int MinAllowed { get; set; } = 1;
        public int MaxAllowed { get; set; } = 1;

        // Foreign key
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        // Navigation property
        public ICollection<ProductOption> Options { get; set; } = new List<ProductOption>();
    }
}