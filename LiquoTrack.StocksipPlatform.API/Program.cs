using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.CommandServices;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.QueryServices;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Converters.JSON;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Persistence.MongoDB.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Converters.JSON;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Interfaces.ASP.Configuration.Namings;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.Namings;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Seeding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

// Registers the value object mapping for all contexts
GlobalMongoMappingHelper.RegisterAllBoundedContextMappings();

// Create a builder for the web application
var builder = WebApplication.CreateBuilder(args);

// Add relevant things about endpoints, controllers, and swagger
builder.Services.AddRouting(o => o.LowercaseUrls = true);
builder.Services.AddControllers(o => o.Conventions.Add(new KebabCaseRouteNamingConvention()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

// Register MongoDB conventions for camel case naming
CamelCaseFieldNamingConvention.UseCamelCaseNamingConvention();

// Add Cors policy to allow all origins, methods, and headers
builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowAllPolicy", p =>
        p.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Configure Swagger/OpenAPI
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "LiquoTrack.StocksipPlatform.API",
        Version     = "v1",
        Description = "StockSip Platform API",
        TermsOfService = new Uri("https://stocksip.com/tos"),
        Contact = new OpenApiContact { Name = "StockSip", Email = "contact@stocksip.com" },
        License = new OpenApiLicense
        {
            Name = "Apache 2.0",
            Url  = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html")
        }
    });
    
    // Enable Annotations for Swagger
    o.EnableAnnotations();
});

// Dependency Injection

// Registers the MongoDB client as a singleton service
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config["MongoDB:ConnectionString"];
    return new MongoClient(connectionString);
});

// Add service for MongoDB client
builder.Services.AddSingleton<AppDbContext>();

// Bounded Context Shared
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<DatabaseSeeder>();

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new AccountIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new UserIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new EmailJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ImageUrlJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ProductIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new InventoryIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new PurchaseOrderIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new CatalogIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new MoneyJsonConverter());
});

// Bounded Context Inventory Management
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IBrandQueryService, BrandQueryService>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductQueryService, ProductQueryService>();
builder.Services.AddScoped<IProductCommandService, ProductCommandService>();

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new EBrandNamesJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new EProductStatesJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new EProductTypesJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ProductContentJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ProductExpirationDateJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ProductMinimumStockJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ProductNameJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ProductStockJsonConverter());
});

// Bounded Context Alerts and Notifications

// Bounded Context Procurement Ordering

// Bounded Context Order Management

// Bounded Context Profile Management

// Bounded Context IAM

// Bounded Context Payment and Subscriptions

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Build the application
var app = builder.Build();

// Use the seeding methods when the application starts
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply CORS Policy
app.UseCors("AllowAllPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();