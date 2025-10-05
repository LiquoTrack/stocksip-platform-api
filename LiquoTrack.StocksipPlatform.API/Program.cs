using System.Diagnostics;
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
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Tokens.JWT.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Converters.JSON;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Interfaces.ASP.Configuration.Namings;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.Namings;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Seeding;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using System.Text.Json;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Authentication;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Pipeline.Middleware.Extensions;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.External.ACL;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.CommandServices;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.ACL.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.CommandHandlers;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Persistence.Repositories;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Tokens.JWT.Configuration;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Application.CommandServices;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Application.QueryServices;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Infrastructure.Converters.JSON;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Infrastructure.Persistence.MongoDB.Repositories;
using GoogleAuthService = LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.Services.GoogleAuthService;

// Register MongoDB mappings
GlobalMongoMappingHelper.RegisterAllBoundedContextMappings();

// Create a builder for the web application
var builder = WebApplication.CreateBuilder(args);

// Add logger
var logger = LoggerFactory.Create(config =>
{
    config.AddConsole();
    config.AddDebug();
}).CreateLogger<Program>();

// Add services to the container
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
        policyBuilder =>
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

// Dependency Injection

// Register Google Auth Services with fully qualified names
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();

// Register Google SignIn Command Handler and its dependencies
builder.Services.AddScoped<GoogleSignInCommandHandler>();

// Register External Auth Service (Google) with a fully qualified name
builder.Services.AddScoped<IExternalAuthService, 
    LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google.GoogleAuthService>();

// Register the custom token validator first
builder.Services.AddSingleton<ISecurityTokenValidator, CustomGoogleTokenValidator>();

// Register Google Token Validator with configuration
builder.Services.AddScoped<IGoogleTokenValidator>(sp => 
    new CustomGoogleTokenValidatorAdapter(
        sp.GetRequiredService<ISecurityTokenValidator>(),
        sp.GetRequiredService<ILogger<CustomGoogleTokenValidatorAdapter>>(),
        sp.GetRequiredService<IConfiguration>()));

// Registers the MongoDB client as a singleton service
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var cs = builder.Configuration["MongoDB:ConnectionString"];
    return string.IsNullOrEmpty(cs) 
        ? throw new InvalidOperationException("MongoDB connection string is not configured") 
        : new MongoClient(cs);
});

// Register IMongoDatabase
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var dbName = builder.Configuration["MongoDB:DatabaseName"];
    return string.IsNullOrEmpty(dbName) 
        ? throw new InvalidOperationException("MongoDB database name is not configured") 
        : client.GetDatabase(dbName);
});

// Add service for MongoDB client
builder.Services.AddSingleton<AppDbContext>();

//
// Bounded Context Shared
//
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

//
// Authentication Bounded Context
//
builder.Services.AddScoped<IUserRepository, UserRepository>();

// JWT Configuration
builder.Services.Configure<TokenSettings>(
    builder.Configuration.GetSection("Jwt"));

// Register Token Service
builder.Services.AddScoped<ITokenService, TokenService>();

// Register Hashing Service
builder.Services.AddScoped<IHashingService, HashingService>();

// Register token validator and authentication services
builder.Services.AddSingleton<CustomGoogleTokenValidator>();
builder.Services.AddScoped<ISecurityTokenValidator>(sp => sp.GetRequiredService<CustomGoogleTokenValidator>());
builder.Services.AddScoped<IGoogleTokenValidator, CustomGoogleTokenValidatorAdapter>();

// Using a fully qualified name to resolve ambiguity
builder.Services.AddScoped<IExternalAuthService, 
    LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google.GoogleAuthService>();

// JWT Configuration
builder.Services.Configure<LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Tokens.JWT.Configuration.TokenSettings>(
    builder.Configuration.GetSection("Jwt"));

// Register Token Service
builder.Services.AddScoped<ITokenService, TokenService>();

// Register Hashing Service
builder.Services.AddScoped<IHashingService, HashingService>();

// Register token validator and authentication services
builder.Services.AddSingleton<CustomGoogleTokenValidator>();
builder.Services.AddScoped<ISecurityTokenValidator>(sp => sp.GetRequiredService<CustomGoogleTokenValidator>());
builder.Services.AddScoped<IGoogleTokenValidator, CustomGoogleTokenValidatorAdapter>();

// Register user services
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

//
// Bounded Context Inventory Management
//
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IBrandQueryService, BrandQueryService>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductQueryService, ProductQueryService>();
builder.Services.AddScoped<IProductCommandService, ProductCommandService>();

builder.Services.AddScoped<ICareGuideRepository, CareGuideRepository>();
builder.Services.AddScoped<ICareGuideQueryService, CareGuideQueryService>();
builder.Services.AddScoped<ICareGuideCommandService, CareGuideCommandService>();

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
    options.JsonSerializerOptions.Converters.Add(new WarehouseAddressJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new WarehouseCapacityJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new WarehouseTemperatureJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new CareGuideJsonConverter());
});

//
// Bounded Context Alerts and Notifications
//

//
// Bounded Context Procurement Ordering
//

//
// Bounded Context Order Management
//

//
// Bounded Context Profile Management
//
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IProfileQueryService, ProfileQueryService>();
builder.Services.AddScoped<IProfileCommandService, ProfileCommandService>();

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new PersonContactNumberJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new PersonNameJsonConverter());
});

//
// Bounded Context Payment and Subscriptions
//
builder.Services.AddScoped<IPlanQueryService, PlanQueryService>();
builder.Services.AddScoped<IAccountQueryService, AccountQueryService>();
builder.Services.AddScoped<IAccountCommandService, AccountCommandService>();
builder.Services.AddScoped<IBusinessCommandService, BusinessCommandService>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

builder.Services.AddScoped<IPaymentAndSubscriptionsFacade, PaymentAndSubscriptionsFacade>();

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
        options.RequireHttpsMetadata = false; // For development only

        var jwtSettings = builder.Configuration.GetSection("Jwt");
                // JWT configuration
                var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? 
                    throw new InvalidOperationException("JWT Secret no está configurado"));

                // Get JWT settings from configuration
                var validateIssuer = jwtSettings.GetValue<bool>("ValidateIssuer");
                var validateAudience = jwtSettings.GetValue<bool>("ValidateAudience");
                var validateLifetime = jwtSettings.GetValue<bool>("ValidateLifetime");
                var validateIssuerSigningKey = jwtSettings.GetValue<bool>("ValidateIssuerSigningKey");
                var requireExpirationTime = jwtSettings.GetValue<bool>("RequireExpirationTime", true);
                var clockSkew = TimeSpan.FromMinutes(jwtSettings.GetValue<int>("ClockSkew", 30));
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Validate issuer
                    ValidateIssuer = validateIssuer,
                    ValidIssuers = [jwtSettings["Issuer"], "https://accounts.google.com"],
                    
                    // Validate audience
                    ValidateAudience = validateAudience,
                    ValidAudiences = [jwtSettings["Audience"], jwtSettings["ClientId"]],
                    
                    // Validate token lifetime
                    ValidateLifetime = validateLifetime,
                    ClockSkew = clockSkew,
                    
                    // Configure issuer signing key
                    ValidateIssuerSigningKey = validateIssuerSigningKey,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    
                    // Other settings
                    RequireExpirationTime = requireExpirationTime,
                    RequireSignedTokens = jwtSettings.GetValue<bool>("RequireSignedTokens", false),
                    
                    // Configuración de claims
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role
                };
                
                // Log the JWT validation parameters for debugging
                logger.LogInformation("JWT Validation Parameters:");
                logger.LogInformation("- ValidateIssuer: {ValidateIssuer}", validateIssuer);
                logger.LogInformation("- ValidateAudience: {ValidateAudience}", validateAudience);
                logger.LogInformation("- ValidateLifetime: {ValidateLifetime}", validateLifetime);
                logger.LogInformation("- ValidateIssuerSigningKey: {ValidateIssuerSigningKey}", validateIssuerSigningKey);
                logger.LogInformation("- ClockSkew: {ClockSkew}", clockSkew);   
                
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Headers.Authorization.ToString();
                if (!string.IsNullOrEmpty(accessToken) && accessToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = accessToken["Bearer ".Length..].Trim();
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var tokenLogger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                var identity = context.Principal?.Identity as ClaimsIdentity;
                
                tokenLogger.LogInformation("=== TOKEN VALIDADO ===");
                tokenLogger.LogInformation("Usuario autenticado: {User}", context.Principal?.Identity?.Name);
                tokenLogger.LogInformation("Autenticado: {IsAuthenticated}", context.Principal?.Identity?.IsAuthenticated);
                
                tokenLogger.LogInformation("=== CLAIMS DEL TOKEN ===");
                Debug.Assert(context.Principal != null, "context.Principal != null");
                
                foreach (var claim in context.Principal.Claims)
                {
                    tokenLogger.LogInformation("Claim - Tipo: {Type}, Valor: {Value}", claim.Type, claim.Value);

                    if (claim.Type is not ("role" or "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"))
                        continue;
                    
                    tokenLogger.LogInformation("Rol encontrado: {Role}", claim.Value);
                    
                    if (context.Principal.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == claim.Value))
                        continue;
                    
                    var newClaim = new Claim(ClaimTypes.Role, claim.Value);
                    identity?.AddClaim(newClaim);
                    tokenLogger.LogInformation("Añadido claim de rol: {Role}", claim.Value);
                }
                
                var hasAdminRole = context.Principal.IsInRole("Admin");
                tokenLogger.LogInformation("¿Tiene rol Admin? {HasAdminRole}", hasAdminRole);
                
                tokenLogger.LogInformation("=== FIN DE VALIDACIÓN DE TOKEN ===");
                
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                var localLogger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                localLogger.LogError(context.Exception, "Error de autenticación");
                
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.Headers.Append("Token-Expired", "true");
                    logger.LogError("El token ha expirado");
                }
                
                switch (context.Exception)
                {
                    case SecurityTokenInvalidIssuerException:
                        logger.LogError("Emisor del token no válido. Se esperaba: {ValidIssuers}", context.Options.TokenValidationParameters.ValidIssuers);
                        break;
                    
                    case SecurityTokenInvalidAudienceException:
                        logger.LogError("Audiencia del token no válida. Se esperaba: {Join}", string.Join(", ", context.Options.TokenValidationParameters.ValidAudiences));
                        break;
                }

                return Task.CompletedTask;
            }
        };
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? throw new InvalidOperationException();
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? throw new InvalidOperationException();
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
                    new("http://schemas.microsoft.com/identity/claims/identityprovider", "Google"),
                    new Claim(ClaimTypes.Role, "User")
                };

                var identity = new ClaimsIdentity(claims, context.Scheme.Name);
                context.Principal = new ClaimsPrincipal(identity);
                
                context.Success();
            }
            catch (Exception ex)
            {
                var localLogger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                localLogger.LogError(ex, "Error creating authentication ticket");
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

// Configure the HTTP request pipeline
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

// Add middleware in the correct order
app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Configure endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseAuthentication();

// Configure the Authentication HTTP request pipeline.
app.UseRequestAuthorization();

app.MapControllers();

app.Run();