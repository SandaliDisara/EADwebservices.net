using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ProductService
{
    private readonly IMongoCollection<Product> _products;

    public ProductService(IConfiguration config)
    {
        var client = new MongoClient(config.GetSection("MongoDBSettings:ConnectionString").Value);
        var database = client.GetDatabase(config.GetSection("MongoDBSettings:DatabaseName").Value);
        _products = database.GetCollection<Product>(config.GetSection("MongoDBSettings:ProductCollectionName").Value);
    }

    // Get all products
    public async Task<List<Product>> GetAllProductsAsync() => await _products.Find(p => true).ToListAsync();

    // Get product by Id
    public async Task<Product> GetProductByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            throw new FormatException("Invalid ObjectId format.");
        }

        return await _products.Find(p => p.Id == objectId.ToString()).FirstOrDefaultAsync();
    }

    // Create a new product
    public async Task CreateProductAsync(Product product) => await _products.InsertOneAsync(product);

    // Update a product by Id
    public async Task UpdateProductAsync(string id, Product updatedProduct)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            throw new FormatException("Invalid ObjectId format.");
        }

        updatedProduct.Id = id; // Ensure the ID remains unchanged

        var filter = Builders<Product>.Filter.Eq(p => p.Id, objectId.ToString()); // Filter by the original Id
        await _products.ReplaceOneAsync(filter, updatedProduct); // Replace the document but keep the original Id
    }

    // Delete a product by Id
    public async Task DeleteProductAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            throw new FormatException("Invalid ObjectId format.");
        }

        var filter = Builders<Product>.Filter.Eq(p => p.Id, objectId.ToString());
        await _products.DeleteOneAsync(filter);
    }

    // New method: Get products by VendorID
    public async Task<List<Product>> GetProductsByVendorIdAsync(string vendorId)
    {
        if (!ObjectId.TryParse(vendorId, out var objectId))
        {
            throw new FormatException("Invalid ObjectId format.");
        }

        var filter = Builders<Product>.Filter.Eq(p => p.VendorID, objectId.ToString());
        return await _products.Find(filter).ToListAsync();
    }

    // New method: Get active products (optional, based on IsActive flag)
    public async Task<List<Product>> GetActiveProductsAsync()
    {
        var filter = Builders<Product>.Filter.Eq(p => p.IsActive, true);
        return await _products.Find(filter).ToListAsync();
    }
}
