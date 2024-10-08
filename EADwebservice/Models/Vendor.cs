using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

/// <summary>
/// Represents a vendor in the e-commerce application.
/// </summary>
/// 
/// <remarks>
/// The Vendor class stores details about a vendor, including the vendor's name,
/// description, average ranking based on customer feedback, and a list of comments
/// made by customers. This class facilitates managing vendor-related information.
/// </remarks>

public class Vendor
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; } // Vendor Name
    public string Description { get; set; } // Vendor Description
    public decimal AverageRanking { get; set; } = 0; // Calculated average ranking

    public List<Comment> Comments { get; set; } = new List<Comment>(); // List of comments from customers
}

/// <summary>
/// Represents a comment made by a customer about a vendor.
/// </summary>
/// 
/// <remarks>
/// The Comment class holds the details of a customer's feedback, including the
/// customer's ID, name, the text of the comment, and a ranking score.
/// </remarks>

public class Comment
{
    public string CustomerId { get; set; } // Customer who made the comment
    public string CustomerName { get; set; } // Name of the customer
    public string Text { get; set; } // Comment text
    public int Ranking { get; set; } // Ranking score (1-5)
}
