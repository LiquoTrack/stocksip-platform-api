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
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Application.CommandServices;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Application.QueryServices;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Infrastructure.Converters.JSON;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Infrastructure.Persistence.MongoDB.Repositories;

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
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IProfileQueryService, ProfileQueryService>();
builder.Services.AddScoped<IProfileCommandService, ProfileCommandService>();

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new PersonContactNumberJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new PersonNameJsonConverter());
});

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

// Google credentials from settings
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
        options.SaveToken = true;
        options.IncludeErrorDetails = true;

        var jwtSettings = builder.Configuration.GetSection("Jwt");
        
        // JWT configuration
        var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? 
            throw new InvalidOperationException("JWT Secret no está configurado"));
            
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validate issuer (our server)
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            
            // Validate audience (our application)
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            
            // Validate token lifetime
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),
            
            // Configure issuer signing key
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            
            // Configuración de claims
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
        
        // Only validate local JWT tokens, not Google tokens
        options.SecurityTokenValidators.Clear();
        options.SecurityTokenValidators.Add(new JwtSecurityTokenHandler());
        
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
                var identity = context.Principal?.Identity as ClaimsIdentity;
                
                logger.LogInformation("=== TOKEN VALIDADO ===");
                logger.LogInformation("Usuario autenticado: {User}", context.Principal?.Identity?.Name);
                logger.LogInformation("Autenticado: {IsAuthenticated}", context.Principal?.Identity?.IsAuthenticated);
                
                logger.LogInformation("=== CLAIMS DEL TOKEN ===");
                foreach (var claim in context.Principal.Claims)
                {
                    logger.LogInformation("Claim - Tipo: {Type}, Valor: {Value}", claim.Type, claim.Value);
                    
                    if (claim.Type == "role" || claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                    {
                        logger.LogInformation("Rol encontrado: {Role}", claim.Value);
                        if (!context.Principal.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == claim.Value))
                        {
                            var newClaim = new Claim(ClaimTypes.Role, claim.Value);
                            identity?.AddClaim(newClaim);
                            logger.LogInformation("Añadido claim de rol: {Role}", claim.Value);
                        }
                    }
                }
                
                var hasAdminRole = context.Principal.IsInRole("Admin");
                logger.LogInformation("¿Tiene rol Admin? {HasAdminRole}", hasAdminRole);
                
                logger.LogInformation("=== FIN DE VALIDACIÓN DE TOKEN ===");
                
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
                    logger.LogError($"Emisor del token no válido. Se esperaba: {context.Options.TokenValidationParameters.ValidIssuers}");
                }
                
                if (context.Exception is SecurityTokenInvalidAudienceException)
                {
                    logger.LogError($"Audiencia del token no válida. Se esperaba: {string.Join(", ", context.Options.TokenValidationParameters.ValidAudiences)}");
                }
                
                return Task.CompletedTask;
            }
        };
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

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LiquoTrack API",
        Version = "v1",
        Description = "API for LiquoTrack Stock Management System"
    });
    
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
        c.OAuthUsePkce(); 
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

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();