using EADwebservice.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Register Services for DI
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<BackOfficerService>();
builder.Services.AddSingleton<VendorService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<NotificationService>();

// Add CORS policy to allow all origins (or restrict to specific IPs if needed)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin() // Allow requests from any IP address or domain
                     .AllowAnyMethod() // Allow any HTTP method (GET, POST, PUT, DELETE, etc.)
                     .AllowAnyHeader(); // Allow any header
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable CORS for the React app
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();