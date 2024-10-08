using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;


/// <summary>
/// Represents a shopping cart for a customer in the e-commerce application.
/// </summary>
/// 
/// <remarks>
/// The Cart class holds the items a customer intends to purchase,
/// associating them with a specific customer through CustomerId.
/// </remarks>

public class Cart
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } // Unique cart identifier

    public string CustomerId { get; set; } // Customer ID to associate the cart with a customer

    public List<CartItem> Items { get; set; } = new List<CartItem>(); // List of items in the cart, each linked to a vendor
}

/// <summary>
/// Represents an item in the shopping cart.
/// </summary>
/// 
/// <remarks>
/// The CartItem class defines the details of each product in the cart,
/// including its ID, name, quantity, price, and associated vendor.
/// </remarks>
/// 
public class CartItem
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductId { get; set; } // Product ID to associate with the cart item

    public string ProductName { get; set; } // Product name
    public int Quantity { get; set; } // Quantity of the product in the cart
    public decimal Price { get; set; } // Price of the product

    [BsonRepresentation(BsonType.ObjectId)]
    public string VendorId { get; set; } // Vendor ID associated with the product
}
