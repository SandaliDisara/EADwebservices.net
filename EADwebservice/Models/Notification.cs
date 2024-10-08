using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

/// <summary>
/// Represents a notification in the system.
/// </summary>
/// 
/// <remarks>
/// The Notification class is used to store information about different types of notifications,
/// including their type, title, body content, associated sender, and order details.
/// Notifications are typically used to inform users of events, alerts, or updates.
/// </remarks>

namespace YourNamespace.Models // Replace with your actual namespace
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? NotificationId { get; set; }  // MongoDB will generate the Id if null

        public string Type { get; set; } // Type of notification (e.g., "Info", "Warning", "Error")
        public string Title { get; set; } // Title of the notification
        public string Body { get; set; } // Body content of the notification

        public string? SenderId { get; set; } // Not required for all notifications
        public string? OrderId { get; set; } // Not required for all notifications

        public DateTime DateOfCreation { get; set; } = DateTime.UtcNow; // Default to current UTC time
    }
}
