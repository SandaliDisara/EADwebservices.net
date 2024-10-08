using EADwebservice.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EADwebservice.Services
{
    public interface IOrderService
    {
        // Get all orders (for admin or CSR to see all orders)
        Task<List<Order>> GetAllOrdersAsync();

        // Get a specific order by ID
        Task<Order> GetOrderByIdAsync(string orderId);

        // Get all orders for a specific customer
        Task<List<Order>> GetOrdersByCustomerIdAsync(string customerId);

        // Get all orders containing products associated with a specific vendor
        Task<List<Order>> GetOrdersByVendorIdAsync(string vendorId); // New method for vendors to see their product orders

        // Create a new order
        Task CreateOrderAsync(Order order);

        // Update the status of an entire order
        Task<bool> UpdateOrderStatusAsync(string orderId, string status);

        // Update the status of a product within an order
        Task<bool> UpdateProductStatusAsync(string orderId, string productId, string productStatus);

        // Delete an order
        Task<bool> DeleteOrderAsync(string orderId);
    }
}
