using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

public class VendorService
{
    private readonly IMongoCollection<Vendor> _vendors;

    public VendorService(IConfiguration config)
    {
        var client = new MongoClient(config.GetSection("MongoDBSettings:ConnectionString").Value);
        var database = client.GetDatabase(config.GetSection("MongoDBSettings:DatabaseName").Value);
        _vendors = database.GetCollection<Vendor>(config.GetSection("MongoDBSettings:VendorCollectionName").Value);
    }

    // Get all vendors
    public async Task<List<Vendor>> GetAllVendorsAsync() =>
        await _vendors.Find(v => true).ToListAsync();

    // Get vendor by Id
    public async Task<Vendor> GetVendorByIdAsync(string id) =>
        await _vendors.Find(v => v.Id == id).FirstOrDefaultAsync();

    // Create a new vendor (Administrator only)
    public async Task CreateVendorAsync(Vendor vendor) =>
        await _vendors.InsertOneAsync(vendor);

    // Add or update a comment from a customer
    public async Task AddOrUpdateCommentAsync(string vendorId, Comment comment)
    {
        var vendor = await GetVendorByIdAsync(vendorId);
        if (vendor == null) throw new Exception("Vendor not found");

        var existingComment = vendor.Comments?.Find(c => c.CustomerId == comment.CustomerId);

        // Update existing comment or add a new one
        if (existingComment != null)
        {
            existingComment.Text = comment.Text; // Update comment text
            existingComment.Ranking = comment.Ranking; // Update ranking
        }
        else
        {
            vendor.Comments.Add(comment); // Add new comment
        }

        // Recalculate the average ranking
        vendor.AverageRanking = CalculateAverageRanking(vendor.Comments);

        var filter = Builders<Vendor>.Filter.Eq(v => v.Id, vendorId);
        await _vendors.ReplaceOneAsync(filter, vendor);
    }

    // Delete a vendor by Id
    public async Task DeleteVendorAsync(string id)
    {
        var filter = Builders<Vendor>.Filter.Eq(v => v.Id, id);
        var result = await _vendors.DeleteOneAsync(filter);
        if (result.DeletedCount == 0) throw new Exception("Vendor not found or already deleted.");
    }

    // Private helper to calculate the average ranking
    private decimal CalculateAverageRanking(List<Comment> comments)
    {
        if (comments == null || comments.Count == 0) return 0;
        return (decimal)comments.Average(c => c.Ranking);
    }
}
