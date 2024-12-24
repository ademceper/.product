public interface IProductService
{
    Task<bool> CreateProductAsync(Product product);
    Task<bool> UpdateProductAsync(string id, Product product);
    Task<bool> DeleteProductAsync(string id);
}