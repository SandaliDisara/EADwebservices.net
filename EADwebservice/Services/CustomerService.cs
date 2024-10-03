using EADwebservice.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using EADwebservice.Dtos.Customer;

namespace EADwebservice.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IMongoCollection<Customer> _customers;

        // Constructor to initialize MongoDB collection
        public CustomerService(IConfiguration config)
        {
            var client = new MongoClient(config.GetSection("MongoDBSettings:ConnectionString").Value);
            var database = client.GetDatabase(config.GetSection("MongoDBSettings:DatabaseName").Value);
            _customers = database.GetCollection<Customer>(config.GetSection("MongoDBSettings:CustomerCollectionName").Value);
        }

        // Get all customers from MongoDB
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _customers.Find(customer => true).ToListAsync();
        }

        // Get a single customer by ID
        public async Task<Customer> GetCustomerByIdAsync(string id)
        {
            return await _customers.Find(customer => customer.Id == id).FirstOrDefaultAsync();
        }

        // Get customer by email
        public async Task<Customer> GetCustomerByEmailAsync(string email) // Add this method implementation
        {
            return await _customers.Find(customer => customer.Email == email).FirstOrDefaultAsync();
        }

        // Get all inactive customers (for admin/CSR to review)
        public async Task<List<Customer>> GetInactiveCustomersAsync()
        {
            return await _customers.Find(customer => customer.IsActive == false).ToListAsync();
        }

        // Register a new customer (inactive by default)
        public async Task CreateCustomerAsync(Customer customer)
        {
            customer.IsActive = false; // New accounts are inactive by default
            await _customers.InsertOneAsync(customer);
        }

        // Update a customer’s details
        public async Task<bool> UpdateCustomerAsync(string id, CustomerUpdateDto customerDto)
        {
            var customer = await GetCustomerByIdAsync(id);
            if (customer == null) return false;

            var update = Builders<Customer>.Update
                .Set(c => c.FirstName, customerDto.FirstName)
                .Set(c => c.LastName, customerDto.LastName)
                .Set(c => c.Address, customerDto.Address)
                .Set(c => c.PhoneNumber, customerDto.PhoneNumber);

            var result = await _customers.UpdateOneAsync(c => c.Id == id, update);
            return result.ModifiedCount > 0;
        }

        // Delete a customer
        public async Task<bool> DeleteCustomerAsync(string id)
        {
            var result = await _customers.DeleteOneAsync(c => c.Id == id);
            return result.DeletedCount > 0;
        }

        // Authenticate a customer by email and password
        public async Task<Customer> AuthenticateCustomerAsync(string email, string password)
        {
            var customer = await _customers.Find<Customer>(c => c.Email == email).FirstOrDefaultAsync();

            // If customer is found and password is correct, check if account is active
            if (customer != null && BCrypt.Net.BCrypt.Verify(password, customer.Password))
            {
                if (!customer.IsActive)
                {
                    return null; // Return null if the account is not active
                }
                return customer; // Return customer if everything matches
            }

            return null; // Return null if email or password is incorrect
        }

        // Activate a customer (called by admin/CSR)
        public async Task<bool> ActivateCustomerAsync(string id)
        {
            var customer = await GetCustomerByIdAsync(id);
            if (customer == null) return false;

            var update = Builders<Customer>.Update.Set(c => c.IsActive, true);
            var result = await _customers.UpdateOneAsync(c => c.Id == id, update);

            return result.ModifiedCount > 0;
        }
    }
}
