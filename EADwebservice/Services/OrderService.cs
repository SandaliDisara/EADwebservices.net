using MongoDB.Bson;
using EADwebservice.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace EADwebservice.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<Order> _orders;

        public OrderService(IConfiguration config)
        {
            var client = new MongoClient(config.GetSection("MongoDBSettings:ConnectionString").Value);
            var database = client.GetDatabase(config.GetSection("MongoDBSettings:DatabaseName").Value);
            _orders = database.GetCollection<Order>(config.GetSection("MongoDBSettings:OrderCollectionName").Value);
        }

        // Helper method to parse ObjectId
        private ObjectId? TryParseObjectId(string id)
        {
            return ObjectId.TryParse(id, out var objectId) ? objectId : (ObjectId?)null;
        }

        // Create a new order
        public async Task CreateOrderAsync(Order order)
        {
            await _orders.InsertOneAsync(order);
        }

        // Get all orders for admins or CSRs
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orders.Find(order => true).ToListAsync();
        }

        // Get a specific order by ID
        public async Task<Order> GetOrderByIdAsync(string orderId)
        {
            var objectId = TryParseObjectId(orderId);
            if (objectId == null)
                return null; // Invalid ObjectId, return null or handle appropriately

            return await _orders.Find(order => order.Id == objectId.ToString()).FirstOrDefaultAsync();
        }

        // Get all orders for a customer
        public async Task<List<Order>> GetOrdersByCustomerIdAsync(string customerId)
        {
            var objectId = TryParseObjectId(customerId);
            if (objectId == null)
                return new List<Order>(); // Invalid ObjectId, return empty list or handle appropriately

            return await _orders.Find(order => order.CustomerId == objectId.ToString()).ToListAsync();
        }

        // Get orders for a specific vendor (filtering by VendorId)
        public async Task<List<Order>> GetOrdersByVendorIdAsync(string vendorId)
        {
            // Find all orders where any product has the given vendorId
            var filter = Builders<Order>.Filter.ElemMatch(o => o.Products, p => p.VendorId == vendorId);
            var orders = await _orders.Find(filter).ToListAsync();

            // Filter out products that don't belong to the vendor
            foreach (var order in orders)
            {
                order.Products = order.Products.Where(p => p.VendorId == vendorId).ToList();
            }

            return orders;
        }

        // Update order status
        public async Task<bool> UpdateOrderStatusAsync(string orderId, string status)
        {
            var objectId = TryParseObjectId(orderId);
            if (objectId == null)
                return false; // Invalid ObjectId, return false or handle appropriately

            var filter = Builders<Order>.Filter.Eq(o => o.Id, objectId.ToString());
            var update = Builders<Order>.Update.Set(o => o.OrderStatus, status);

            var result = await _orders.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        // Update product status within an order
        public async Task<bool> UpdateProductStatusAsync(string orderId, string productId, string productStatus)
        {
            var orderObjectId = TryParseObjectId(orderId);
            var productObjectId = TryParseObjectId(productId);

            if (orderObjectId == null || productObjectId == null)
                return false; // Invalid ObjectId, return false or handle appropriately

            var filter = Builders<Order>.Filter.And(
                Builders<Order>.Filter.Eq(o => o.Id, orderObjectId.ToString()),
                Builders<Order>.Filter.Eq("Products.ProductId", productObjectId.ToString())
            );

            var update = Builders<Order>.Update.Set("Products.$.ProductStatus", productStatus);

            var result = await _orders.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        // Delete an order
        public async Task<bool> DeleteOrderAsync(string orderId)
        {
            var objectId = TryParseObjectId(orderId);
            if (objectId == null)
                return false; // Invalid ObjectId, return false or handle appropriately

            var result = await _orders.DeleteOneAsync(order => order.Id == objectId.ToString());
            return result.DeletedCount > 0;
        }
    }
}
