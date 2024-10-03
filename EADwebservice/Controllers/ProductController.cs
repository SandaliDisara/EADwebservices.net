using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAllProducts() => Ok(await _productService.GetAllProductsAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(string id) => Ok(await _productService.GetProductByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult> CreateProduct([FromBody] Product product)
    {
        await _productService.CreateProductAsync(product);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(string id, [FromBody] Product updatedProduct)
    {
        // Use the Id from the URL and don't check against the body
        await _productService.UpdateProductAsync(id, updatedProduct);
        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(string id)
    {
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }
}
