using EADwebservice.Dtos.Customer;
using EADwebservice.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EADwebservice.Services
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllCustomersAsync();
        Task<Customer> GetCustomerByIdAsync(string id);
        Task<Customer> GetCustomerByEmailAsync(string email);  // Add this method
        Task CreateCustomerAsync(Customer customer);
        Task<bool> UpdateCustomerAsync(string id, CustomerUpdateDto customerDto);
        Task<bool> DeleteCustomerAsync(string id);
        Task<Customer> AuthenticateCustomerAsync(string email, string password);
        Task<List<Customer>> GetInactiveCustomersAsync();
        Task<bool> ActivateCustomerAsync(string id);
    }
}
