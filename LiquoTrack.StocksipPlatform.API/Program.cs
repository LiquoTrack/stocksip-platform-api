using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Converters.JSON;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Interfaces.ASP.Configuration.Namings;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.Namings;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

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

// Registers the value object mapping for all contexts
GlobalMongoMappingHelper.RegisterAllBoundedContextMappings();

// Registers the MongoDB client as a singleton service
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config["MongoDB:ConnectionString"];
    return new MongoClient(connectionString);
});

// Add service por MongoDB client
builder.Services.AddSingleton<AppDbContext>();

// Bounded Context Shared
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new AccountIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new UserIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new EmailJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ImageUrlJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ProductIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new InventoryIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new PurchaseOrderIdJsonConverter());
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI();
}

// Apply CORS Policy
app.UseCors("AllowAllPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();