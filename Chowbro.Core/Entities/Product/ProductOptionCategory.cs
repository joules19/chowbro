using Chowbro.Core.Entities;
using Chowbro.Core.Entities.Product;
using static Chowbro.Core.Enums.Product;

public class ProductOptionCategory : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }

    public bool IsRequired { get; set; }
    public OptionSelectionType SelectionType { get; set; } // Single, Multiple
    public int? MinOptions { get; set; } // For multiple selection
    public int? MaxOptions { get; set; } // For multiple selection

    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public ICollection<ProductOption> Options { get; set; } = new List<ProductOption>();
}
