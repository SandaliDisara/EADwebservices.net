using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

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

public class Comment
{
    public string CustomerId { get; set; } // Customer who made the comment
    public string CustomerName { get; set; } // Name of the customer
    public string Text { get; set; } // Comment text
    public int Ranking { get; set; } // Ranking score (1-5)
}
