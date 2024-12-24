public class ProductVariantAttribute
{
    public string Id { get; set; }
    public string ProductVariantId { get; set; }
    public string AttributeName { get; set; }
    public string AttributeValue { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }

    public ProductVariant ProductVariant { get; set; }
}