public class VariantDto
{
    public string VariantName { get; set; }
    public string VariantValue { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountedPrice { get; set; }
    public int StockQuantity { get; set; }
    public string SKU { get; set; }
    public string ImageUrl { get; set; }
    public List<AttributeDto> Attributes { get; set; }
}