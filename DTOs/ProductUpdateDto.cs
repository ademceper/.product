public class ProductUpdateDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public string Info { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountedPrice { get; set; }
    public decimal OriginalPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsFeatured { get; set; }
    public string CategoryId { get; set; }
    public string UpdatedBy { get; set; }
    public string SearchIndex { get; set; }
    public bool IsSearchable { get; set; }
}