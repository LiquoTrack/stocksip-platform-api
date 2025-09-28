using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.CommandServices;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.QueryServices;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Converters.JSON;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Persistence.MongoDB.Repositories;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.QueryServices;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.Converters.JSON;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.Persistence.MongoDB.Repositories;
using System.Security.Claims;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.CommandServices;
using LiquoTrack.StocksipPlatform.API.Security.TokenValidators;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Hashing;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Token;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.QueryServices;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Hashing.BCrypt.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Persistence.Repositories;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Tokens.JWT.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Converters.JSON;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Interfaces.ASP.Configuration.Namings;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.Namings;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Seeding;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using System.Text.Json;

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

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins(
                    "https://localhost:7164",
                    "http://localhost:5283",
                    "https://localhost:44355"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});


// Dependency Injection

// Registers the MongoDB client as a singleton service
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var cs = builder.Configuration["MongoDB:ConnectionString"];
    if (string.IsNullOrEmpty(cs))
    {
        throw new InvalidOperationException("MongoDB connection string is not configured");
    }
    return new MongoClient(cs);
});

// Register IMongoDatabase
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var dbName = builder.Configuration["MongoDB:DatabaseName"];
    if (string.IsNullOrEmpty(dbName))
    {
        throw new InvalidOperationException("MongoDB database name is not configured");
    }
    return client.GetDatabase(dbName);
});

// Add service for MongoDB client
builder.Services.AddSingleton<AppDbContext>();

// Bounded Context Shared
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<DatabaseSeeder>();

// Authentication Bounded Context
builder.Services.AddScoped<IUserRepository, UserRepository>();

// JWT Configuration
builder.Services.Configure<LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Tokens.JWT.Configuration.TokenSettings>(
    builder.Configuration.GetSection("Jwt"));

// Register Token Service
builder.Services.AddScoped<ITokenService, TokenService>();

// Register Hashing Service
builder.Services.AddScoped<IHashingService, HashingService>();

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, MongoUnitOfWork>();

// Register token validator and authentication services
builder.Services.AddSingleton<CustomGoogleTokenValidator>();
builder.Services.AddScoped<ISecurityTokenValidator>(sp => sp.GetRequiredService<CustomGoogleTokenValidator>());
builder.Services.AddScoped<IGoogleTokenValidator, CustomGoogleTokenValidatorAdapter>();
builder.Services.AddScoped<IExternalAuthService, GoogleAuthService>();

// Register user services
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

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
builder.Services.AddScoped<IPlanQueryService, PlanQueryService>();
builder.Services.AddScoped<IAccountQueryService, AccountQueryService>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

builder.Services.Configure<JsonOptions>(options =>
{
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


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Obtener las credenciales de Google desde la configuración
var googleAuthSection = builder.Configuration.GetSection("Authentication:Google");
var googleClientId = googleAuthSection["ClientId"] ?? 
    throw new InvalidOperationException("Google ClientId no está configurado en appsettings.json");
var googleClientSecret = googleAuthSection["ClientSecret"] ?? 
    throw new InvalidOperationException("Google ClientSecret no está configurado en appsettings.json");

// Configure authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Solo para desarrollo
        options.SaveToken = true;
        options.IncludeErrorDetails = true;
        
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        var secret = jwtSettings["Secret"];
        
        // Configuración para tokens JWT estándar
        if (!string.IsNullOrEmpty(secret))
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero,
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role
            };
            
            options.Authority = jwtSettings["Issuer"];
        }
        
        // Configuración para validar el token
        options.TokenValidationParameters.ValidAudiences = new[]
        {
            builder.Configuration["Jwt:Audience"],
            builder.Configuration["Authentication:Google:ClientId"]
        };
        
        options.TokenValidationParameters.ValidIssuers = new[]
        {
            builder.Configuration["Jwt:Issuer"],
            "https://accounts.google.com"
        };
        
        // Configuración para tokens de Google
        options.Authority = "https://accounts.google.com";
        options.TokenValidationParameters.ValidateIssuerSigningKey = false; // Desactivar validación de firma para Google
        // Custom token validation to handle array claims
        options.TokenValidationParameters.IssuerSigningKey = null; // Disable signature validation for Google tokens
        options.TokenValidationParameters.ValidateIssuerSigningKey = false;
        
        // Custom token validation to handle array claims in JWT
        options.SecurityTokenValidators.Clear();
        options.SecurityTokenValidators.Add(new CustomGoogleTokenValidator());
        
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Headers.Authorization.ToString();
                if (!string.IsNullOrEmpty(accessToken) && accessToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = accessToken.Substring("Bearer ".Length).Trim();
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Token validado para el usuario: {User}", context.Principal?.Identity?.Name);
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(context.Exception, "Error de autenticación");
                
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                    logger.LogError("El token ha expirado");
                }
                
                if (context.Exception is SecurityTokenInvalidIssuerException)
                {
                    logger.LogError($"Emisor del token no válido. Se esperaba: {context.Options.TokenValidationParameters.ValidIssuer}");
                }
                
                if (context.Exception is SecurityTokenInvalidAudienceException)
                {
                    logger.LogError($"Audiencia del token no válida. Se esperaba: {context.Options.TokenValidationParameters.ValidAudience}");
                }
                
                return Task.CompletedTask;
            }
        };
        // Configuración adicional de autenticación si es necesaria
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        options.CallbackPath = "/signin-google";
        options.AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        options.TokenEndpoint = "https://oauth2.googleapis.com/token";
        options.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
        
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        
        options.SaveTokens = true;
        
        options.Events.OnCreatingTicket = async context =>
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);
                
                var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                response.EnsureSuccessStatusCode();
                
                var user = await response.Content.ReadFromJsonAsync<JsonElement>();
                
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.GetString("id") ?? string.Empty),
                    new(ClaimTypes.Name, user.GetString("name") ?? string.Empty),
                    new(ClaimTypes.Email, user.GetString("email") ?? string.Empty),
                    new(ClaimTypes.GivenName, user.GetString("given_name") ?? string.Empty),
                    new(ClaimTypes.Surname, user.GetString("family_name") ?? string.Empty),
                    new("http://schemas.microsoft.com/identity/claims/identityprovider", "Google")
                };
                
                // Add role claims if needed
                claims.Add(new Claim(ClaimTypes.Role, "User"));
                
                var identity = new ClaimsIdentity(claims, context.Scheme.Name);
                context.Principal = new ClaimsPrincipal(identity);
                
                context.Success();
            }
            catch (Exception ex)
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Error creating authentication ticket");
                context.Fail(ex);
            }
        };
    });

// Configurar Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LiquoTrack API",
        Version = "v1",
        Description = "API for LiquoTrack Stock Management System"
    });
    
    // Add JWT Authentication
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT Bearer token (without 'Bearer ' prefix)",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };
    
    c.AddSecurityDefinition("Bearer", securityScheme);
    
    // Add Security Requirement
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    
    // Add OAuth2 Configuration
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/v2/auth"),
                TokenUrl = new Uri("https://oauth2.googleapis.com/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID" },
                    { "profile", "Profile" },
                    { "email", "Email" }
                }
            }
        }
    });
    
    // Habilitar anotaciones XML para documentación
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
    }
});

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
    app.UseDeveloperExceptionPage();
    
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LiquoTrack API V1");
        c.OAuthClientId("520776661353-aq0nbie37i8742tnn0167ak4bdadk2cu.apps.googleusercontent.com");
        c.OAuthAppName("LiquoTrack API - Swagger");
        c.OAuthUsePkce(); // Habilita PKCE (recomendado)
        c.OAuth2RedirectUrl("https://localhost:7164/swagger/oauth2-redirect.html");
    });
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Use CORS with the correct policy name
app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();