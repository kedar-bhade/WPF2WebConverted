using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Interfaces;
using OrderManagementSystem.Data;
using OrderManagementSystem.Services;
using OrderManagementSystem.Mapping;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Order Management System API",
        Version = "v1",
        Description = "A comprehensive API for managing orders, customers, products, and employees",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Order Management System",
            Email = "support@ordermanagementsystem.com"
        }
    });
});

// Database Configuration
builder.Services.AddDbContext<OrderManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Dependency Injection
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Database initialization and connection test
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();
        
        Console.WriteLine("Testing database connection...");
        Console.WriteLine($"Connection string: {builder.Configuration.GetConnectionString("DefaultConnection")}");
        
        // Test connection
        var canConnect = await context.Database.CanConnectAsync();
        Console.WriteLine($"Can connect to database: {canConnect}");
        
        if (!canConnect)
        {
            Console.WriteLine("Cannot connect to database. Please check your connection string and SQL Server instance.");
            Console.WriteLine("Make sure SQL Server Express is running and the NORTHWIND database exists.");
        }
        else
        {
            // Ensure database is created with the correct schema
            await context.Database.EnsureCreatedAsync();
            Console.WriteLine("Database schema created successfully.");
            
            // Try to get order count
            try
            {
                var orderCount = await context.Orders.CountAsync();
                Console.WriteLine($"Connected to SQL Server database successfully. Found {orderCount} orders.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error querying orders: {ex.Message}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error connecting to database: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
}

// Configure the HTTP request pipeline.
// CORS must be configured FIRST, before any other middleware
app.UseCors("AllowAll");

// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Management System API v1");
    c.RoutePrefix = string.Empty; // Set Swagger UI as the default page
    c.DocumentTitle = "Order Management System API Documentation";
});

// Disable HTTPS redirection for development to avoid CORS issues
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

// No redirect - let the API handle the root
app.MapGet("/", () => "Order Management System API is running. Visit /swagger for documentation.");

app.Run();
