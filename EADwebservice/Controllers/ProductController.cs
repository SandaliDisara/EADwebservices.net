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

    // Get all products
    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    // Get product by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(string id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    // Get products by Vendor ID
    [HttpGet("vendor/{vendorId}")]
    public async Task<ActionResult<List<Product>>> GetProductsByVendorId(string vendorId)
    {
        var products = await _productService.GetProductsByVendorIdAsync(vendorId);
        if (products == null || products.Count == 0)
        {
            return NotFound($"No products found for VendorID: {vendorId}");
        }
        return Ok(products);
    }

    // Create a new product
    [HttpPost]
    public async Task<ActionResult> CreateProduct([FromBody] Product product)
    {
        if (product == null || string.IsNullOrEmpty(product.VendorID))
        {
            return BadRequest("Product and VendorID are required.");
        }

        await _productService.CreateProductAsync(product);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    // Update an existing product by ID
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(string id, [FromBody] Product updatedProduct)
    {
        if (string.IsNullOrEmpty(updatedProduct.VendorID))
        {
            return BadRequest("VendorID is required for updating a product.");
        }

        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        await _productService.UpdateProductAsync(id, updatedProduct);
        return NoContent();
    }

    // Delete a product by ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        await _productService.DeleteProductAsync(id);
        return NoContent();
    }
}
