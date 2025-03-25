using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chowbro.Infrastructure.Entities.Vendor
{

    public class ProductOption : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();  
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PriceModifier { get; set; }

        // Foreign key
        public Guid CategoryId { get; set; }
        public ProductOptionCategory Category { get; set; }
    }

}