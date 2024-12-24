using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Product product)
    {
        var success = await _productService.CreateProductAsync(product);
        if (!success) return BadRequest("Failed to create product.");
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Product product)
    {
        var success = await _productService.UpdateProductAsync(id, product);
        if (!success) return BadRequest("Failed to update product.");
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _productService.DeleteProductAsync(id);
        if (!success) return BadRequest("Failed to delete product.");
        return NoContent();
    }
}