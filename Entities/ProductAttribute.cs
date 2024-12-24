public class ProductAttribute
{
    public string Id { get; set; }
    public string ProductId { get; set; }
    public string? AttributeName { get; set; }
    public string AttributeValue { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public Product Product { get; set; }
}