using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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
    public string Password { get; set; } // Store hashed passwords for security

    [BsonElement("role")]
    public string Role { get; set; }  // Role like "Administrator", "Vendor", "CSR"

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;  // To mark if the account is active or not
}
