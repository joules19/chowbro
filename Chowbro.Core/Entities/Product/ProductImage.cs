namespace Chowbro.Core.Entities.Product;

public class ProductImage : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public string Caption { get; set; }
    public string Url { get; set; }

    public bool IsMain { get; set; }

    public string ImageUrl { get; set; }
    public string PublicId { get; set; } // Cloudinary public ID
    public int DisplayOrder { get; set; }

}