public class ProductCreateDto
{
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
    public string CreatedBy { get; set; }
    public string SearchIndex { get; set; }
    public bool IsSearchable { get; set; }
    public List<VariantDto> Variants { get; set; }
    public List<ImageDto> Images { get; set; }
    public List<AttributeDto> Attributes { get; set; }
}