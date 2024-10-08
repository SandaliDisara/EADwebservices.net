using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    // POST: api/cart/{customerId}/add
    [HttpPost("{customerId}/add")]
    public async Task<IActionResult> AddToCart(string customerId, [FromBody] CartItem cartItem)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Invalid cart item data" });

        try
        {
            // Ensure VendorId is passed
            if (string.IsNullOrEmpty(cartItem.VendorId))
                return BadRequest(new { message = "VendorId is required" });

            await _cartService.AddToCartAsync(customerId, cartItem);
            return Ok(new { message = "Item added to cart successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while adding the item to the cart", error = ex.Message });
        }
    }

    // PUT: api/cart/{customerId}/update
    [HttpPut("{customerId}/update")]
    public async Task<IActionResult> UpdateCartItem(string customerId, [FromBody] CartItem updatedItem)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Invalid cart item data" });

        try
        {
            await _cartService.UpdateCartItemAsync(customerId, updatedItem);
            return Ok(new { message = "Cart item updated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating the cart item", error = ex.Message });
        }
    }

    // DELETE: api/cart/{customerId}/delete/{productId}
    [HttpDelete("{customerId}/delete/{productId}")]
    public async Task<IActionResult> RemoveFromCart(string customerId, string productId)
    {
        try
        {
            await _cartService.RemoveFromCartAsync(customerId, productId);
            return Ok(new { message = "Item removed from cart successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while removing the item from the cart", error = ex.Message });
        }
    }

    // GET: api/cart/{customerId}
    [HttpGet("{customerId}")]
    public async Task<IActionResult> GetCartByCustomerId(string customerId)
    {
        try
        {
            var cart = await _cartService.GetCartByCustomerIdAsync(customerId);
            if (cart == null)
                return NotFound(new { message = "Cart not found" });

            return Ok(cart.Items);  // Return the items from the cart
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the cart", error = ex.Message });
        }
    }
}
