using Chowbro.Core.Entities;

public class ProductOption : BaseEntity
{
    public string Name { get; set; }
    public decimal AdditionalPrice { get; set; }
    public bool IsAvailable { get; set; } = true;
    public Guid OptionCategoryId { get; set; }
    public int DisplayOrder { get; set; }
    public ProductOptionCategory OptionCategory { get; set; }
}