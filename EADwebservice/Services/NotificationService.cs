using EADwebservice.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using YourNamespace.Models;

namespace EADwebservice.Services
{
    public class NotificationService
    {
        private readonly IMongoCollection<Notification> _notifications;

        // Constructor to initialize MongoDB collection
        public NotificationService(IConfiguration config)
        {
            var client = new MongoClient(config.GetSection("MongoDBSettings:ConnectionString").Value);
            var database = client.GetDatabase(config.GetSection("MongoDBSettings:DatabaseName").Value);
            _notifications = database.GetCollection<Notification>("Notifications");
        }

        // Create a new notification
        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            await _notifications.InsertOneAsync(notification); // MongoDB will generate the NotificationId
            return notification; // Return the created notification
        }

        // Get all notifications
        public async Task<List<Notification>> GetAllNotificationsAsync()
        {
            return await _notifications.Find(notification => true).ToListAsync();
        }

        // Get a notification by ID
        public async Task<Notification> GetNotificationByIdAsync(string id)
        {
            return await _notifications.Find(notification => notification.NotificationId == id).FirstOrDefaultAsync();
        }

        // Delete a notification by ID
        public async Task<bool> DeleteNotificationAsync(string id)
        {
            var result = await _notifications.DeleteOneAsync(notification => notification.NotificationId == id);
            return result.DeletedCount > 0; // Return true if a document was deleted
        }
    }
}
