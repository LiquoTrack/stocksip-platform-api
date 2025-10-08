using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.CommandServices;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Hashing;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Token;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.QueryServices;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Hashing.BCrypt.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Persistence.Repositories;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Tokens.JWT.Configuration;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Tokens.JWT.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Pipeline.Middleware.Extensions;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.CommandServices;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.OutboundServices.FileStorage;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.QueryServices;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Converters.JSON;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.FileStorage.Cloudinary.Services;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Persistence.MongoDB.Repositories;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.CommandServices;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.QueryServices;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.Converters.JSON;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.Persistence.MongoDB.Repositories;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Application.Internal.ACL;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Application.Internal.CommandServices;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Application.Internal.OutBoundServices.FileStorage;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Application.QueryServices;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Infrastructure.Converters.JSON;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Infrastructure.FileStorage.Cloudinary.Services;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Infrastructure.Persistence.MongoDB.Repositories;
using LiquoTrack.StocksipPlatform.API.Security.TokenValidators;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Converters.JSON;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Interfaces.ASP.Configuration.Namings;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.Namings;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Seeding;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Reflection;
using System.Text.Json;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Authentication;
using AppGoogleAuthService = LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.Services.GoogleAuthService;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.External.ACL;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.ACL.Services;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Interfaces.ACL;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.FileStorage.Cloudinary.Configuration;


GlobalMongoMappingHelper.RegisterAllBoundedContextMappings();

var builder = WebApplication.CreateBuilder(args);

// Logger
var loggerFactory = LoggerFactory.Create(config =>
{
    config.AddConsole();
    config.AddDebug();
});
var logger = loggerFactory.CreateLogger("Program");

// Configuration shortcuts
var configuration = builder.Configuration;
var env = builder.Environment;

// Services 
builder.Services.AddRouting(o => o.LowercaseUrls = true);
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()));
builder.Services.AddEndpointsApiExplorer();

// HttpContextAccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policyBuilder =>
    {
        policyBuilder.WithOrigins(
                "https://localhost:7164",
                "http://localhost:5283",
                "https://localhost:44355")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var cs = configuration["MongoDB:ConnectionString"];
    if (string.IsNullOrWhiteSpace(cs))
        throw new InvalidOperationException("MongoDB connection string is not configured (MongoDB:ConnectionString).");
    return new MongoClient(cs);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var dbName = configuration["MongoDB:DatabaseName"];
    if (string.IsNullOrWhiteSpace(dbName))
        throw new InvalidOperationException("MongoDB database name is not configured (MongoDB:DatabaseName).");
    return client.GetDatabase(dbName);
});

builder.Services.AddSingleton<AppDbContext>();
builder.Services.AddScoped<DatabaseSeeder>();

builder.Services.Configure<JsonOptions>(options =>
{
    // Shared converters
    options.JsonSerializerOptions.Converters.Add(new AccountIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new UserIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new EmailJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ImageUrlJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ProductIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new InventoryIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new PurchaseOrderIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new CatalogIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new MoneyJsonConverter());

    // Inventory converters
    options.JsonSerializerOptions.Converters.Add(new EBrandNamesJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new EProductStatesJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new EProductTypesJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ProductContentJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ProductExpirationDateJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ProductMinimumStockJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ProductNameJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ProductStockJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new WarehouseAddressJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new WarehouseCapacityJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new WarehouseTemperatureJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new CareGuideJsonConverter());

    // Profile converters
    options.JsonSerializerOptions.Converters.Add(new PersonContactNumberJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new PersonNameJsonConverter());

    // Payment converters
    options.JsonSerializerOptions.Converters.Add(new BusinessEmailJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new BusinessNameJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new EAccountRoleJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new EAccountStatusesJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new EPaymentFrequencyJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new EPlanTypeJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ESubscriptionStatusJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new PlanLimitsJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new RucJsonConverter());
});

builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();

// Google Auth Service 
builder.Services.AddScoped<IGoogleAuthService, AppGoogleAuthService>();
builder.Services.AddScoped<GoogleSignInCommandHandler>();
builder.Services.AddScoped<IExternalAuthService, LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google.GoogleAuthService>();

// Bounded context Inventory
builder.Services.AddScoped<IInventoryImageService, InventoryImageService>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IBrandQueryService, BrandQueryService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductQueryService, ProductQueryService>();
builder.Services.AddScoped<IProductCommandService, ProductCommandService>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IWarehouseCommandService, WarehouseCommandService>();
builder.Services.AddScoped<IWarehouseQueryService, WarehouseQueryService>();
builder.Services.AddScoped<ICareGuideRepository, CareGuideRepository>();
builder.Services.AddScoped<ICareGuideQueryService, CareGuideQueryService>();
builder.Services.AddScoped<ICareGuideCommandService, CareGuideCommandService>();

// Bounded context Profile
builder.Services.AddScoped<IProfilesImageService, ProfilesImageService>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IProfileQueryService, ProfileQueryService>();
builder.Services.AddScoped<IProfileCommandService, ProfileCommandService>();
builder.Services.AddScoped<IProfileContextFacade, ProfileContextFacade>();

// Bounded context Payment & Subscriptions
builder.Services.AddScoped<IPlanQueryService, PlanQueryService>();
builder.Services.AddScoped<IAccountQueryService, AccountQueryService>();
builder.Services.AddScoped<IAccountCommandService, AccountCommandService>();
builder.Services.AddScoped<IBusinessCommandService, BusinessCommandService>();
builder.Services.AddScoped<IBusinessQueryService, BusinessQueryService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<IPaymentAndSubscriptionsFacade, PaymentAndSubscriptionsFacade>();

builder.Services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));

// Authentication: hashing & token service
builder.Services.AddScoped<IHashingService, HashingService>(); // BCrypt
builder.Services.AddScoped<ITokenService, TokenService>();

// Token settings 
builder.Services.Configure<TokenSettings>(configuration.GetSection("Jwt"));

builder.Services.AddSingleton<CustomGoogleTokenValidator>();
builder.Services.AddSingleton<ISecurityTokenValidator>(sp => sp.GetRequiredService<CustomGoogleTokenValidator>());
builder.Services.AddScoped<IGoogleTokenValidator, CustomGoogleTokenValidatorAdapter>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LiquoTrack API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Use 'Bearer {token}'"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var googleClientId = configuration["Authentication:Google:ClientId"];
var googleClientSecret = configuration["Authentication:Google:ClientSecret"];

if (!string.IsNullOrWhiteSpace(googleClientId))
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = "https://accounts.google.com";
        options.RequireHttpsMetadata = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuers = new[] { "accounts.google.com", "https://accounts.google.com" },
            ValidateAudience = true,
            ValidAudience = googleClientId,
            ValidateLifetime = configuration.GetValue<bool>("Jwt:ValidateLifetime", true),
            ClockSkew = TimeSpan.FromMinutes(configuration.GetValue<int>("Jwt:ClockSkew", 5)),
            
            ValidateIssuerSigningKey = false,
            RequireExpirationTime = true,
            RequireSignedTokens = true
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = authHeader["Bearer ".Length..].Trim();
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var tokenLogger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                tokenLogger.LogInformation("Token validado para {sub}", context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                var localLogger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                localLogger.LogError(context.Exception, "Error de autenticación JWT");
                // Si expiró, añade header para cliente
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.Headers.Append("Token-Expired", "true");
                }
                return Task.CompletedTask;
            }
        };
    });
}
else
{
    logger.LogWarning("Authentication:Google:ClientId no configurado. Las validaciones de tokens de Google NO estarán activas.");
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    try
    {
        await seeder.SeedAsync();
        logger.LogInformation("Database seeding finalizado correctamente.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error durante DatabaseSeeder.SeedAsync(). Revisa la configuración/seed data.");
    }
}

if (app.Environment.IsDevelopment() || configuration.GetValue<bool>("EnableSwaggerInProduction", false))
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LiquoTrack API V1"));
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowSpecificOrigins");

if (!string.IsNullOrWhiteSpace(googleClientId))
{
    app.UseAuthentication();
    app.UseAuthorization();
}

app.Map("/error", (HttpContext http) =>
{
    var exFeature = http.Features.Get<IExceptionHandlerFeature>();
    if (exFeature?.Error != null)
    {
        var err = exFeature.Error;
        http.Response.StatusCode = 500;
        return Results.Problem(detail: err.Message, title: "Unhandled exception");
    }
    return Results.Problem("Unknown error");
});

app.MapControllers();

try
{
    logger.LogInformation("Starting application. Environment: {env}", env.EnvironmentName);
    app.Run();
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Host terminated unexpectedly");
    throw;
}
