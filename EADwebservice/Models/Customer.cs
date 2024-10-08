using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

/// <summary>
/// Represents a customer in the e-commerce application.
/// </summary>
/// 
/// <remarks>
/// The Customer class stores the personal information of customers,
/// including their name, email, password, address, and contact details.
/// It also maintains the account status and the date when the account was created.
/// </remarks>

namespace EADwebservice.Models
{
    public class Customer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }  // MongoDB will auto-generate the ID if it's null

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }  // In production, passwords should be hashed
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; } = false; // Set to false by default
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
