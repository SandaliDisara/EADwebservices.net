using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

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

    public class OrderProduct
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string ProductStatus { get; set; } = "Ordered"; // Status for individual products
    }
}
