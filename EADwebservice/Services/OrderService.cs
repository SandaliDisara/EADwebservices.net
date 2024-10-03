using MongoDB.Bson;
using EADwebservice.Models;
using MongoDB.Driver;
using System.Collections.Generic;
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

        // Create a new order
        public async Task CreateOrderAsync(Order order)
        {
            await _orders.InsertOneAsync(order);
        }

        // Get all orders
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orders.Find(order => true).ToListAsync();
        }

        // Get a specific order by ID (convert string to ObjectId)
        public async Task<Order> GetOrderByIdAsync(string orderId)
        {
            if (!ObjectId.TryParse(orderId, out var objectId))
            {
                return null; // or throw an exception
            }

            return await _orders.Find(order => order.Id == objectId.ToString()).FirstOrDefaultAsync();
        }

        // Get all orders for a customer (convert string to ObjectId)
        public async Task<List<Order>> GetOrdersByCustomerIdAsync(string customerId)
        {
            if (!ObjectId.TryParse(customerId, out var objectId))
            {
                return new List<Order>(); // or throw an exception
            }

            return await _orders.Find(order => order.CustomerId == objectId.ToString()).ToListAsync();
        }

        // Update order status (convert string to ObjectId)
        public async Task<bool> UpdateOrderStatusAsync(string orderId, string status)
        {
            if (!ObjectId.TryParse(orderId, out var objectId))
            {
                return false; // or throw an exception
            }

            var filter = Builders<Order>.Filter.Eq(o => o.Id, objectId.ToString());
            var update = Builders<Order>.Update.Set(o => o.OrderStatus, status);

            var result = await _orders.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        // Update product status within an order
        public async Task<bool> UpdateProductStatusAsync(string orderId, string productId, string productStatus)
        {
            if (!ObjectId.TryParse(orderId, out var orderObjectId) || !ObjectId.TryParse(productId, out var productObjectId))
            {
                return false; // or throw an exception
            }

            var filter = Builders<Order>.Filter.Eq(o => o.Id, orderObjectId.ToString()) & Builders<Order>.Filter.ElemMatch(o => o.Products, p => p.ProductId == productObjectId.ToString());
            var update = Builders<Order>.Update.Set(o => o.Products[-1].ProductStatus, productStatus);

            var result = await _orders.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        // Delete an order (convert string to ObjectId)
        public async Task<bool> DeleteOrderAsync(string orderId)
        {
            if (!ObjectId.TryParse(orderId, out var objectId))
            {
                return false; // or throw an exception
            }

            var result = await _orders.DeleteOneAsync(order => order.Id == objectId.ToString());
            return result.DeletedCount > 0;
        }
    }
}
