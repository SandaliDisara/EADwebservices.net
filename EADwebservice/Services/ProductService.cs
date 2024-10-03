using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ProductService
{
    private readonly IMongoCollection<Product> _products;

    public ProductService(IConfiguration config)
    {
        // Fix the way you're retrieving the connection string and other config values
        var client = new MongoClient(config.GetSection("MongoDBSettings:ConnectionString").Value);
        var database = client.GetDatabase(config.GetSection("MongoDBSettings:DatabaseName").Value);
        _products = database.GetCollection<Product>(config.GetSection("MongoDBSettings:ProductCollectionName").Value);
    }

    public async Task<List<Product>> GetAllProductsAsync() => await _products.Find(p => true).ToListAsync();
    public async Task<Product> GetProductByIdAsync(string id) => await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
    public async Task CreateProductAsync(Product product) => await _products.InsertOneAsync(product);
    public async Task UpdateProductAsync(string id, Product updatedProduct)
    {
        // Ensure the Id in updatedProduct is not altered
        updatedProduct.Id = id; // Preserve the existing Id

        var filter = Builders<Product>.Filter.Eq(p => p.Id, id); // Filter by the original Id
        await _products.ReplaceOneAsync(filter, updatedProduct); // Replace the document but keep the original Id
    }

    public async Task DeleteProductAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            throw new FormatException("Invalid ObjectId format.");
        }

        var filter = Builders<Product>.Filter.Eq(p => p.Id, objectId.ToString());
        await _products.DeleteOneAsync(filter);
    }
}
