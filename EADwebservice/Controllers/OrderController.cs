using EADwebservice.Models;
using EADwebservice.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EADwebservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Create a new order
        [HttpPost]
        public async Task<ActionResult> CreateOrder(Order order)
        {
            await _orderService.CreateOrderAsync(order);
            return CreatedAtAction(nameof(GetOrderById), new { orderId = order.Id }, order);
        }

        // Get order by ID
        [HttpGet("{orderId}")]
        public async Task<ActionResult<Order>> GetOrderById(string orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null) return NotFound();

            return Ok(order);
        }

        // Get orders by CustomerId
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByCustomerId(string customerId)
        {
            var orders = await _orderService.GetOrdersByCustomerIdAsync(customerId);
            return Ok(orders);
        }

        // Get all orders for administrators and CSRs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            if (orders == null || orders.Count == 0)
                return NotFound("No orders found");

            return Ok(orders);
        }

        // New API to get orders containing vendor products by vendorId
        [HttpGet("vendor/{vendorId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByVendorId(string vendorId)
        {
            var orders = await _orderService.GetOrdersByVendorIdAsync(vendorId);
            if (orders == null || orders.Count == 0)
                return NotFound("No orders found for this vendor");

            return Ok(orders);
        }

        // Update order status
        [HttpPut("{orderId}/status")]
        public async Task<ActionResult> UpdateOrderStatus(string orderId, [FromBody] string status)
        {
            var result = await _orderService.UpdateOrderStatusAsync(orderId, status);
            if (!result) return NotFound();

            return Ok();
        }

        // Update product status within an order
        [HttpPut("{orderId}/products/{productId}/status")]
        public async Task<ActionResult> UpdateProductStatus(string orderId, string productId, [FromBody] string productStatus)
        {
            var result = await _orderService.UpdateProductStatusAsync(orderId, productId, productStatus);
            if (!result) return NotFound();

            return Ok();
        }

        // Delete an order
        [HttpDelete("{orderId}")]
        public async Task<ActionResult> DeleteOrder(string orderId)
        {
            var result = await _orderService.DeleteOrderAsync(orderId);
            if (!result) return NotFound();

            return NoContent();
        }
    }
}
