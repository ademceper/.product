public class ProductVariant
{
    public string Id { get; set; }
    public string ProductId { get; set; }
    public string VariantName { get; set; }
    public string VariantValue { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountedPrice { get; set; }
    public int StockQuantity { get; set; }
    public string SKU { get; set; }
    public string ImageUrl { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }

    public Product Product { get; set; }
    public ICollection<ProductVariantAttribute> Attributes { get; set; }
}