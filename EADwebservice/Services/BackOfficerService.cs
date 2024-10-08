using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCrypt.Net; // To hash and verify passwords
using Microsoft.Extensions.Configuration;

public class BackOfficerService
{
    private readonly IMongoCollection<BackOfficer> _backOfficers;

    public BackOfficerService(IConfiguration config)
    {
        var client = new MongoClient(config.GetSection("MongoDBSettings:ConnectionString").Value);
        var database = client.GetDatabase(config.GetSection("MongoDBSettings:DatabaseName").Value);
        _backOfficers = database.GetCollection<BackOfficer>(config.GetSection("MongoDBSettings:BackOfficerCollectionName").Value);
    }

    // Get all back officers
    public async Task<List<BackOfficer>> GetAllBackOfficersAsync()
    {
        try
        {
            return await _backOfficers.Find(backOfficer => true).ToListAsync();
        }
        catch (Exception ex)
        {
            // Log the error
            throw new Exception("Could not fetch back officers", ex);
        }
    }

    // Get back officer by ID
    public async Task<BackOfficer> GetBackOfficerByIdAsync(string id)
    {
        try
        {
            return await _backOfficers.Find(backOfficer => backOfficer.Id == id).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            // Log the error
            throw new Exception("Could not fetch the back officer", ex);
        }
    }

    // Create a new back officer (hash the password)
    public async Task CreateBackOfficerAsync(BackOfficer backOfficer)
    {
        try
        {
            // Hash the password before saving
            backOfficer.Password = BCrypt.Net.BCrypt.HashPassword(backOfficer.Password);
            await _backOfficers.InsertOneAsync(backOfficer);
        }
        catch (Exception ex)
        {
            // Log the error
            throw new Exception("Could not create the back officer", ex);
        }
    }

    // Update an existing back officer
    public async Task UpdateBackOfficerAsync(string id, BackOfficer updatedBackOfficer)
    {
        try
        {
            var filter = Builders<BackOfficer>.Filter.Eq(bo => bo.Id, id);
            updatedBackOfficer.Password = BCrypt.Net.BCrypt.HashPassword(updatedBackOfficer.Password);
            await _backOfficers.ReplaceOneAsync(filter, updatedBackOfficer);
        }
        catch (Exception ex)
        {
            // Log the error
            throw new Exception("Could not update the back officer", ex);
        }
    }

    // Delete back officer
    public async Task DeleteBackOfficerAsync(string id)
    {
        try
        {
            await _backOfficers.DeleteOneAsync(bo => bo.Id == id);
        }
        catch (Exception ex)
        {
            // Log the error
            throw new Exception("Could not delete the back officer", ex);
        }
    }

    // Authenticate back officer using username and password, also check if account is active
    public async Task<BackOfficer> AuthenticateBackOfficerAsync(string username, string password)
    {
        try
        {
            var backOfficer = await _backOfficers.Find(bo => bo.Username == username).FirstOrDefaultAsync();

            // Check if back officer exists, if password matches, and if the account is active
            if (backOfficer == null || !BCrypt.Net.BCrypt.Verify(password, backOfficer.Password) || !backOfficer.IsActive)
            {
                return null; // Invalid credentials or inactive account
            }

            return backOfficer; // Valid credentials
        }
        catch (Exception ex)
        {
            // Log the error
            throw new Exception("Error during authentication", ex);
        }
    }
}
