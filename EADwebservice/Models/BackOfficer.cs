using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

/// <summary>
/// Represents a Back Officer in the system.
/// </summary>
/// 
/// <remarks>
/// The BackOfficer class is used to manage back office users in the application,
/// including administrators, vendors, and customer service representatives (CSR).
/// </remarks>

public class BackOfficer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } // MongoDB generates the Id if null

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("username")]
    public string Username { get; set; }

    [BsonElement("password")]
    public string Password { get; set; } 

    [BsonElement("role")]
    public string Role { get; set; }  // Role like "Administrator", "Vendor", "CSR"

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true; 
}
