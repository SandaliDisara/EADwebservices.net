using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

/// <summary>
/// Represents an order placed by a customer in the e-commerce application.
/// </summary>
/// 
/// <remarks>
/// The Order class stores details about the order, including the customer information,
/// the products ordered, total price, status, and the address for delivery.
/// </remarks>

namespace EADwebservice.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } // Nullable string, MongoDB will generate the Id if null

        [BsonRepresentation(BsonType.ObjectId)]
        public string CustomerId { get; set; } // Reference to the customer who placed the order

        public List<OrderProduct> Products { get; set; } // List of products in the order

        public string Address { get; set; }

        public decimal TotalPrice { get; set; }

        public string OrderStatus { get; set; } = "Pending"; // Default status

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Represents a product in an order.
    /// </summary>
    /// 
    /// <remarks>
    /// The OrderProduct class holds details about each product included in an order,
    /// such as its ID, name, quantity, price, and status. It also associates each product
    /// with the vendor that owns it.
    /// </remarks>
    /// 
    public class OrderProduct
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string ProductStatus { get; set; } = "Ordered"; // Status for individual products

        // New field to associate the product with a vendor
        [BsonRepresentation(BsonType.ObjectId)]
        public string VendorId { get; set; } // Reference to the vendor who owns the product
    }
}
