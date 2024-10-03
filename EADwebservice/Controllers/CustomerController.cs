using EADwebservice.Models;
using EADwebservice.Services;
using Microsoft.AspNetCore.Mvc;
using EADwebservice.Dtos.Customer;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCrypt.Net; // Namespace for BCrypt

namespace EADwebservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // Get all customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        // Get a single customer by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(string id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();

            return Ok(customer);
        }

        // Get all inactive customers (for CSR/Admin)
        [HttpGet("inactive")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetInactiveCustomers()
        {
            var inactiveCustomers = await _customerService.GetInactiveCustomersAsync();
            return Ok(inactiveCustomers);
        }

        // Register a new customer (set IsActive to false by default)
        [HttpPost("register")]
        public async Task<ActionResult> RegisterCustomer(CustomerRegisterDto customerDto)
        {
            // Hash the password using BCrypt
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(customerDto.Password);

            var customer = new Customer
            {
                FirstName = customerDto.FirstName,
                LastName = customerDto.LastName,
                Email = customerDto.Email,
                Password = hashedPassword, // Store the hashed password
                Address = customerDto.Address,
                PhoneNumber = customerDto.PhoneNumber,
                IsActive = false // Default to inactive
            };

            await _customerService.CreateCustomerAsync(customer);
            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
        }

        // Authenticate customer (login)
        [HttpPost("login")]
        public async Task<ActionResult<Customer>> Login(CustomerLoginDto loginDto)
        {
            // Find the customer by email
            var customer = await _customerService.GetCustomerByEmailAsync(loginDto.Email);
            if (customer == null) return Unauthorized("Invalid email or password."); // Invalid email

            // Verify password using BCrypt
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, customer.Password))
            {
                return Unauthorized("Invalid email or password."); // Invalid password
            }

            // Check if the account is active
            if (!customer.IsActive)
            {
                return Unauthorized("Account not activated."); // Account is inactive
            }

            return Ok(customer); // Return customer if everything is correct
        }

        // Activate a customer account (only CSR/Admin can access this)
        [HttpPut("activate/{id}")]
        public async Task<ActionResult> ActivateCustomer(string id)
        {
            var result = await _customerService.ActivateCustomerAsync(id);
            if (!result) return NotFound(); // Return 404 if the customer was not found

            return NoContent(); // Return 204 if successfully activated
        }

        // Update customer information
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCustomer(string id, CustomerUpdateDto customerDto)
        {
            var result = await _customerService.UpdateCustomerAsync(id, customerDto);
            if (!result) return NotFound();

            return NoContent();
        }

        // Delete a customer
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomer(string id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            if (!result) return NotFound();

            return NoContent();
        }
    }
}
