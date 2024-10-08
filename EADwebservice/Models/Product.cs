using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

/// <summary>
/// Represents a product in the e-commerce application.
/// </summary>
/// 
/// <remarks>
/// The Product class stores details about a product, including its name, description,
/// price, category, stock quantity, and associated vendor. It also indicates whether
/// the product is active or not, which can be used for inventory management.
/// </remarks>

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } // Nullable string, MongoDB will generate the Id if null

    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; } = true;

    [BsonRepresentation(BsonType.ObjectId)] // Storing VendorID as an ObjectId
    public string VendorID { get; set; } // New VendorID field to link products to vendors
}
