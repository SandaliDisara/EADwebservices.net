public interface ICartService
{
    // Add a product to the cart (including vendorId and vendorName)
    Task AddToCartAsync(string customerId, CartItem cartItem);

    // Update a product in the cart (including vendorId and vendorName if applicable)
    Task UpdateCartItemAsync(string customerId, CartItem updatedItem);

    // Remove a product from the cart by productId
    Task RemoveFromCartAsync(string customerId, string productId);

    // Get all products in a customer's cart
    Task<Cart> GetCartByCustomerIdAsync(string customerId);
}
