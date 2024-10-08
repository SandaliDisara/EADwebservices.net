using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CartService : ICartService
{
    private readonly IMongoCollection<Cart> _carts;

    public CartService(IConfiguration config)
    {
        var client = new MongoClient(config.GetSection("MongoDBSettings:ConnectionString").Value);
        var database = client.GetDatabase(config.GetSection("MongoDBSettings:DatabaseName").Value);
        _carts = database.GetCollection<Cart>(config.GetSection("MongoDBSettings:CartCollectionName").Value);
    }

    // Add a product to the cart (create new cart if necessary)
    public async Task AddToCartAsync(string customerId, CartItem cartItem)
    {
        var cart = await _carts.Find(c => c.CustomerId == customerId).FirstOrDefaultAsync();

        if (cart == null)
        {
            // Create a new cart if one doesn't exist
            cart = new Cart
            {
                CustomerId = customerId,
                Items = new List<CartItem> { cartItem }  // Include vendorId in the CartItem
            };
            await _carts.InsertOneAsync(cart);
        }
        else
        {
            // Add the item to the existing cart by ProductId
            var existingItem = cart.Items.Find(i => i.ProductId == cartItem.ProductId);
            if (existingItem != null)
            {
                // If the product already exists in the cart, update the quantity and vendorId
                existingItem.Quantity += cartItem.Quantity;
                existingItem.VendorId = cartItem.VendorId;  // Ensure vendorId is stored/updated
            }
            else
            {
                // Otherwise, add the new product with vendorId to the cart
                cart.Items.Add(cartItem);
            }
            await _carts.ReplaceOneAsync(c => c.CustomerId == customerId, cart);
        }
    }

    // Update a product in the cart
    public async Task UpdateCartItemAsync(string customerId, CartItem updatedItem)
    {
        var cart = await _carts.Find(c => c.CustomerId == customerId).FirstOrDefaultAsync();
        if (cart == null) throw new Exception("Cart not found");

        var existingItem = cart.Items.Find(i => i.ProductId == updatedItem.ProductId);
        if (existingItem != null)
        {
            // Update the existing item with the new quantity, price, and vendorId
            existingItem.Quantity = updatedItem.Quantity;
            existingItem.Price = updatedItem.Price;
            existingItem.VendorId = updatedItem.VendorId;  // Ensure vendorId is updated
            await _carts.ReplaceOneAsync(c => c.CustomerId == customerId, cart);
        }
        else
        {
            throw new Exception("Item not found in cart");
        }
    }

    // Remove a product from the cart
    public async Task RemoveFromCartAsync(string customerId, string productId)
    {
        var cart = await _carts.Find(c => c.CustomerId == customerId).FirstOrDefaultAsync();
        if (cart == null) throw new Exception("Cart not found");

        cart.Items.RemoveAll(i => i.ProductId == productId);
        await _carts.ReplaceOneAsync(c => c.CustomerId == customerId, cart);
    }

    // Get all products in a customer's cart
    public async Task<Cart> GetCartByCustomerIdAsync(string customerId) =>
        await _carts.Find(c => c.CustomerId == customerId).FirstOrDefaultAsync();
}
