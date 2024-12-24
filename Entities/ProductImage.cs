public class ProductImage
{
    public string Id { get; set; }
    public string ProductId { get; set; }
    public string ImageUrl { get; set; }
    public bool IsPrimary { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }

    public Product Product { get; set; }
}