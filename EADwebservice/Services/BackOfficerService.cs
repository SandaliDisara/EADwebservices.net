using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCrypt.Net;  // Make sure to include this

public class BackOfficerService
{
    private readonly IMongoCollection<BackOfficer> _backOfficers;

    public BackOfficerService(IConfiguration config)
    {
        // Initialize the MongoDB client and collection from the config
        var client = new MongoClient(config.GetSection("MongoDBSettings:ConnectionString").Value);
        var database = client.GetDatabase(config.GetSection("MongoDBSettings:DatabaseName").Value);
        _backOfficers = database.GetCollection<BackOfficer>(config.GetSection("MongoDBSettings:BackOfficerCollectionName").Value);
    }

    // Get all BackOfficers
    public async Task<List<BackOfficer>> GetAllBackOfficersAsync() =>
        await _backOfficers.Find(bo => true).ToListAsync();

    // Get a single BackOfficer by Id
    public async Task<BackOfficer> GetBackOfficerByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            throw new FormatException("Invalid ObjectId format.");
        }

        return await _backOfficers.Find(bo => bo.Id == objectId.ToString()).FirstOrDefaultAsync();
    }

    // Create a new BackOfficer
    public async Task CreateBackOfficerAsync(BackOfficer backOfficer)
    {
        // Hash the password before saving (recommended)
        backOfficer.Password = BCrypt.Net.BCrypt.HashPassword(backOfficer.Password);
        await _backOfficers.InsertOneAsync(backOfficer);
    }

    // Update an existing BackOfficer
    public async Task UpdateBackOfficerAsync(string id, BackOfficer updatedBackOfficer)
    {
        // Ensure the Id is not altered
        updatedBackOfficer.Id = id;

        // Hash the password if it's changed
        var existingOfficer = await GetBackOfficerByIdAsync(id);
        if (!BCrypt.Net.BCrypt.Verify(updatedBackOfficer.Password, existingOfficer.Password))
        {
            updatedBackOfficer.Password = BCrypt.Net.BCrypt.HashPassword(updatedBackOfficer.Password);
        }

        var filter = Builders<BackOfficer>.Filter.Eq(bo => bo.Id, id);
        await _backOfficers.ReplaceOneAsync(filter, updatedBackOfficer);
    }

    // Delete a BackOfficer by Id
    public async Task DeleteBackOfficerAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            throw new FormatException("Invalid ObjectId format.");
        }

        var filter = Builders<BackOfficer>.Filter.Eq(bo => bo.Id, objectId.ToString());
        await _backOfficers.DeleteOneAsync(filter);
    }
}
