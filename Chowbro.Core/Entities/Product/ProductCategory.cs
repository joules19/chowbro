﻿using Chowbro.Core.Entities;
using Chowbro.Core.Entities.Product;
using Chowbro.Core.Entities.Vendor;

public class ProductCategory : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsMain { get; set; }
    public string Url { get; set; }

    public Guid VendorId { get; set; }
    public Vendor Vendor { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}