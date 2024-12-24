using Elastic.Clients.Elasticsearch;

public class ProductService : IProductService
{
    private readonly ElasticsearchClient _elasticsearchClient;
    private const string IndexName = "products";

    public ProductService(ElasticsearchClient elasticsearchClient)
    {
        _elasticsearchClient = elasticsearchClient;
    }

    public async Task<bool> CreateProductAsync(Product product)
    {
        var response = await _elasticsearchClient.IndexAsync(product, i => i.Index(IndexName));
        return response.IsValidResponse;
    }

    public async Task<bool> UpdateProductAsync(string id, Product product)
    {
        var response = await _elasticsearchClient.UpdateAsync<Product, Product>(id, u => u
            .Index(IndexName)
            .Doc(product));
        return response.IsValidResponse;
    }

    public async Task<bool> DeleteProductAsync(string id)
    {
        var response = await _elasticsearchClient.DeleteAsync<Product>(id, d => d.Index(IndexName));
        return response.IsValidResponse;
    }
}
