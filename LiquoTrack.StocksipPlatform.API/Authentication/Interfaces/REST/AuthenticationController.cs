using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google.Requests;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google.Responses;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;
using System.Security.Claims;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Transform;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST
{
    [ApiController]
    [Route("api/v1")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerTag("Authentication endpoints")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class AuthenticationController : ControllerBase
    {
        private const int DefaultPageSize = 10;
        private const int MaxPageSize = 50;
        private const string GoogleAuthProvider = "Google";
        private const int TokenExpirationInDays = 7;

        private readonly IExternalAuthService _externalAuth;
        private readonly IUserCommandService _userCommand;
        private readonly IUserQueryService _userQueryService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(
            IExternalAuthService externalAuth,
            IUserCommandService userCommandService,
            IUserQueryService userQueryService,
            IConfiguration configuration,
            ILogger<AuthenticationController> logger)
        {
            _externalAuth = externalAuth ?? throw new ArgumentNullException(nameof(externalAuth));
            _userCommand = userCommandService ?? throw new ArgumentNullException(nameof(userCommandService));
            _userQueryService = userQueryService ?? throw new ArgumentNullException(nameof(userQueryService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("auth/google")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Authenticate with Google",
            Description = "Authenticates a user using Google's OAuth 2.0 ID token"
        )]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> AuthenticateWithGoogle([FromBody] GoogleAuthRequest request)
        {
            _logger.LogInformation("=== Starting Google Authentication ===");
            _logger.LogInformation($"Request received at: {DateTime.UtcNow:u}");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning("Invalid model state. Errors: {Errors}", string.Join(", ", errors));
                return BadRequest(new
                {
                    error = "Invalid request data",
                    details = errors
                });
            }

            if (string.IsNullOrWhiteSpace(request?.IdToken))
            {
                _logger.LogWarning("Empty or null ID token provided");
                return BadRequest(new { error = "ID token is required" });
            }

            _logger.LogInformation("Received Google ID token. Starting validation...");
            _logger.LogDebug($"Client ID from request: {request.ClientId}");

            try
            {
                _logger.LogInformation("Decoding token for debugging...");
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(request.IdToken);

                _logger.LogInformation("Token details:");
                _logger.LogInformation($"Issuer: {jwtToken.Issuer}");
                _logger.LogInformation($"Audience: {string.Join(", ", jwtToken.Audiences)}");
                _logger.LogInformation($"Valid From: {jwtToken.ValidFrom}");
                _logger.LogInformation($"Valid To: {jwtToken.ValidTo}");
                _logger.LogInformation("Claims:");
                foreach (var claim in jwtToken.Claims)
                {
                    _logger.LogInformation($"{claim.Type}: {claim.Value}");
                }

                var configuredClientId = _configuration["Authentication:Google:ClientId"]
                                     ?? _configuration["Google:ClientId"];

                _logger.LogInformation($"Configured Client ID: {configuredClientId}");
                _logger.LogInformation($"Request Client ID: {request.ClientId}");

                if (string.IsNullOrEmpty(configuredClientId))
                {
                    _logger.LogError("Google ClientId is not configured in appsettings.json");
                    return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Server configuration error" });
                }

                if (string.IsNullOrWhiteSpace(configuredClientId))
                {
                    const string errorMessage = "Google ClientId not configured";
                    _logger.LogError(errorMessage);
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        new { error = "Server misconfiguration: Google ClientId missing" }
                    );
                }

                _logger.LogInformation("Validating Google ID token with external auth service...");
                var validationResult = await _externalAuth.ValidateIdTokenAsync(request.IdToken);

                if (!validationResult.Success)
                {
                    _logger.LogWarning("Google token validation failed. Error: {Error}", validationResult.Error);
                    _logger.LogWarning("Validation result details: Success={Success}, Email={Email}, Name={Name}",
                        validationResult.Success, validationResult.Email, validationResult.Name);

                    return Unauthorized(new
                    {
                        error = "Authentication failed",
                        details = validationResult.Error ?? "Unknown error during token validation"
                    });
                }

                _logger.LogInformation("Google token validation successful");
                _logger.LogInformation("User email from token: {Email}", validationResult.Email);
                _logger.LogInformation("User name from token: {Name}", validationResult.Name);

                var user = await GetOrCreateUserAsync(validationResult);
                if (user == null)
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        new { error = "Failed to process user account" }
                    );
                }
                var token = GenerateJwtToken(user);
                _logger.LogInformation("Authentication successful for user: {Email}", user.Email);

                var response = new AuthResponse
                {
                    Token = token,
                    UserId = user.Id.ToString(),
                    Email = user.Email.ToString(),
                    Username = user.Username,
                };

                return Ok(response);
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Network error while validating with Google");
                return StatusCode(
                    StatusCodes.Status502BadGateway,
                    new
                    {
                        error = "Could not connect to Google authentication service",
                        details = httpEx.Message
                    }
                );
            }
            catch (SecurityTokenException stEx)
            {
                _logger.LogWarning(stEx, "Security token validation failed");
                return Unauthorized(new
                {
                    error = "Invalid security token",
                    details = stEx.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during authentication. Error: {ErrorMessage}", ex.Message);
                _logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);

                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner Exception: {InnerException}", ex.InnerException.Message);
                    _logger.LogError("Inner Stack Trace: {InnerStackTrace}", ex.InnerException.StackTrace);
                }

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        error = "An error occurred during authentication",
                        details = ex.Message,
                        stackTrace = ex.StackTrace,
                        innerException = ex.InnerException?.Message
                    }
                );
            }
        }

        /// <summary>
        /// Gets or creates a user from the external authentication result.
        /// </summary>
        /// <param name="validationResult">The external authentication result.</param>
        /// <returns>The user.</returns>
        private async Task<User?> GetOrCreateUserAsync(ExternalAuthResult validationResult)
        {
            try
            {
                if (string.IsNullOrEmpty(validationResult.ProviderUserId) || string.IsNullOrEmpty(validationResult.Email))
                {
                    _logger.LogError("Invalid validation result - missing required fields");
                    return null;
                }

                var users = await _userQueryService.GetUsersByEmailAsync(new GetUserByEmailQuery(validationResult.Email));
                var user = users?.FirstOrDefault();

                if (user == null)
                {
                    _logger.LogInformation("Creating new user for external ID: {ExternalId}", validationResult.ProviderUserId);

                    user = await _userCommand.CreateOrUpdateFromExternalAsync(
                        validationResult.ProviderUserId,
                        validationResult.Email,
                        validationResult.Name);

                    if (user == null)
                    {
                        _logger.LogError("Failed to create user for external ID: {ExternalId}", validationResult.ProviderUserId);
                        return null;
                    }

                    _logger.LogInformation("Created new user with ID: {UserId}", user.Id);
                }
                else
                {
                    _logger.LogDebug("Found existing user with ID: {UserId}", user.Id);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting or creating user for external ID: {ExternalId}", validationResult.ProviderUserId);
                throw;
            }
        }

        /// <summary>
        /// Logs the token details.
        /// </summary>
        /// <param name="idToken">The ID token.</param>
        private void LogTokenDetails(string idToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                if (handler.CanReadToken(idToken))
                {
                    var jwt = handler.ReadJwtToken(idToken);
                    var audFromToken = string.Join(",", jwt.Audiences);
                    var iss = jwt.Issuer;
                    var exp = jwt.ValidTo;

                    _logger.LogDebug("Token details - aud: {aud}, iss: {iss}, exp: {exp}",
                        audFromToken, iss, exp);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse token for logging");
            }
        }

        /// <summary>
        /// Gets a paginated list of users with minimal information
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page (max 50)</param>
        /// <returns>Paginated list of users with metadata</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        [SwaggerOperation(
            Summary = "Get paginated list of users",
            Description = "Retrieves a paginated list of users with minimal information. Requires Admin role.",
            OperationId = "GetUsers"
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(PaginatedResponse<UserListResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = DefaultPageSize)
        {
            _logger.LogInformation("=== INICIO DE SOLICITUD GET /api/v1/users ===");
            _logger.LogInformation("User: {User}", User?.Identity?.Name);
            _logger.LogInformation("IsAuthenticated: {IsAuthenticated}", User?.Identity?.IsAuthenticated);
            _logger.LogInformation("Claims:");
            foreach (var claim in User?.Claims ?? Enumerable.Empty<System.Security.Claims.Claim>())
            {
                _logger.LogInformation($"{claim.Type} = {claim.Value}");
            }
            _logger.LogInformation("Fetching users - Page: {Page}, PageSize: {PageSize}", page, pageSize);

            try
            {
                _logger.LogInformation("Running in test mode - authentication bypassed");

                // Validar parámetros de paginación
                if (page < 1)
                {
                    _logger.LogWarning("Número de página inválido: {Page}", page);
                    return BadRequest(new
                    {
                        error = "Parámetro inválido",
                        message = "El número de página debe ser mayor a 0.",
                        details = new { parameter = "page", value = page }
                    });
                }

                pageSize = Math.Min(Math.Max(1, pageSize), MaxPageSize);

                var users = await _userQueryService.GetAllUsersAsync(HttpContext.RequestAborted);
                var usersList = users?.ToList() ?? new List<User>();
                var totalCount = usersList.Count;
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var paginatedUsers = usersList
                    .OrderBy(u => u.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new UserListResponse
                    {
                        Id = int.Parse(u.Id.ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber),
                        Email = u.Email?.ToString() ?? string.Empty,
                        EmailVerified = true,
                        Provider = GoogleAuthProvider,
                        GoogleSub = u.AccountId.ToString(),
                        CreatedAt = u.CreatedAt,
                        LastLogin = u.UpdateAt,
                        IsDisabled = false
                    })
                    .ToList();

                var response = new PaginatedResponse<UserListResponse>
                {
                    Items = paginatedUsers,
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    HasNextPage = page < totalPages,
                    HasPreviousPage = page > 1
                };

                _logger.LogInformation("Se recuperaron exitosamente {Count} usuarios", paginatedUsers.Count);
                return Ok(response);
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogWarning("Token expirado: {Message}", ex.Message);
                return Unauthorized(new
                {
                    error = "Token expirado",
                    message = "El token de autenticación ha expirado. Por favor, obtén un nuevo token e inténtalo de nuevo.",
                    details = new
                    {
                        token_type = "Bearer",
                        expected_issuer = "https://accounts.google.com",
                        expected_audience = _configuration["Authentication:Google:ClientId"],
                        token_expired_at = ex.Expires != default ? ex.Expires.ToString("yyyy-MM-ddTHH:mm:ssZ") : "unknown"
                    }
                });
            }
            catch (SecurityTokenInvalidAudienceException ex)
            {
                _logger.LogWarning("Audiencia de token inválida: {Message}", ex.Message);
                return Unauthorized(new
                {
                    error = "Token inválido",
                    message = "La audiencia del token no es válida para este recurso.",
                    details = new
                    {
                        token_type = "Bearer",
                        expected_audience = _configuration["Authentication:Google:ClientId"],
                        actual_audience = ex.InvalidAudience ?? "unknown"
                    }
                });
            }
            catch (SecurityTokenInvalidIssuerException ex)
            {
                _logger.LogWarning("Emisor de token inválido: {Message}", ex.Message);
                return Unauthorized(new
                {
                    error = "Token inválido",
                    message = "El emisor del token no es válido.",
                    details = new
                    {
                        token_type = "Bearer",
                        expected_issuer = "https://accounts.google.com",
                        actual_issuer = ex.InvalidIssuer ?? "unknown"
                    }
                });
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning("Token de seguridad inválido: {Message}", ex.Message);
                return Unauthorized(new
                {
                    error = "Token inválido",
                    message = "El token de autenticación proporcionado no es válido.",
                    details = new
                    {
                        token_type = "Bearer",
                        required_scheme = JwtBearerDefaults.AuthenticationScheme,
                        expected_token_format = "JWT",
                        expected_issuer = "https://accounts.google.com",
                        expected_audience = _configuration["Authentication:Google:ClientId"],
                        how_to_authenticate = new[]
                        {
                            "Obtén un token de Google usando OAuth 2.0",
                            "Incluye el token en el encabezado: 'Authorization: Bearer <token>'"
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { error = "An error occurred while retrieving users", details = ex.Message, stackTrace = ex.StackTrace }
                );
            }
        }

        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="user">The user to generate the token for.</param>
        /// <returns>A JWT token string.</returns>
        private string GenerateJwtToken(User user)
        {
            try
            {
                _logger.LogInformation("=== Starting JWT Token Generation ===");
                _logger.LogInformation($"User ID: {user.Id}");
                _logger.LogInformation($"User Email: {user.Email?.ToString() ?? "[No Email]"}");

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = _configuration["Jwt:Secret"];
                if (string.IsNullOrEmpty(key))
                {
                    throw new InvalidOperationException("JWT Secret not configured");
                }

                var keyBytes = Encoding.UTF8.GetBytes(key);
                var expiryMinutes = _configuration.GetValue<int>("Jwt:ExpiryMinutes", 1440);
                var clockSkewMinutes = _configuration.GetValue<int>("Jwt:ClockSkew", 5);

                _logger.LogInformation("JWT Configuration:");
                _logger.LogInformation($"Issuer: {_configuration["Jwt:Issuer"]}");
                _logger.LogInformation($"Audience: {_configuration["Jwt:Audience"]}");
                _logger.LogInformation($"Expiry Minutes: {expiryMinutes}");
                _logger.LogInformation($"Clock Skew Minutes: {clockSkewMinutes}");

                var email = user.Email?.ToString() ?? string.Empty;
                if (email.StartsWith("Email { GetValue = ") && email.EndsWith(" }"))
                {
                    email = email["Email { GetValue = ".Length..^2].Trim();
                }
                var claims = new List<System.Security.Claims.Claim>();
                
                claims.Add(new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
                claims.Add(new System.Security.Claims.Claim(JwtRegisteredClaimNames.Email, email));
                claims.Add(new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                claims.Add(new System.Security.Claims.Claim(ClaimTypes.Name, email));
                claims.Add(new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

                string roleName = "User";
                if (user.UserRole != null)
                {
                    var roleType = user.UserRole.GetType();
                    var nameProperty = roleType.GetProperty("Name");
                    if (nameProperty != null)
                    {
                        roleName = nameProperty.GetValue(user.UserRole)?.ToString() ?? "User";
                    }
                }
                
                claims.Add(new System.Security.Claims.Claim(ClaimTypes.Role, roleName));
                claims.Add(new System.Security.Claims.Claim("role", roleName));

                _logger.LogInformation("JWT Claims:");
                foreach (var claim in claims)
                {
                    _logger.LogInformation($"{claim.Type} = {claim.Value}");
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(keyBytes),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                _logger.LogInformation($"JWT Token Descriptor - Issuer: {tokenDescriptor.Issuer}");
                _logger.LogInformation($"JWT Token Descriptor - Audience: {tokenDescriptor.Audience}");
                _logger.LogInformation($"JWT Token Expires: {tokenDescriptor.Expires}");
                
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                _logger.LogInformation("JWT Token generated successfully");
                _logger.LogDebug("Generated Token: {Token}", tokenString);
                _logger.LogInformation("Token will expire at: {Expiration}", token.ValidTo.ToUniversalTime().ToString("u"));

                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token for user {UserId}", user.Id);
                throw new InvalidOperationException("Error generating authentication token", ex);
            }
        }

        /// <summary>
        /// Signs in a user.
        /// </summary>
        /// <param name="signInResource">The sign-in resource.</param>
        /// <returns>The authentication response.</returns>
        [HttpPost("sign-in")]
        [AllowAnonymous]
        [SwaggerOperation(
        Summary = "Sign in",
        Description = "Sign in a user",
        OperationId = "SignIn")]
        [SwaggerResponse(StatusCodes.Status200OK, "The user was authenticated", typeof(AuthResponse))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The sign-in process has failed", typeof(string))]
        public async Task<IActionResult> SignIn([FromBody] SignInResource signInResource)
        {
            try
            {
                var signInCommand = SignInCommandFromResourceAssembler.ToCommandFromResource(signInResource);
                var user = await _userCommand.Handle(signInCommand);
                
                if (user == null)
                    return Unauthorized("Invalid username or password");
                var token = GenerateJwtToken(user);
                
                var response = AuthResponse.Create(
                    token,
                    user.Id,
                    user.Email.ToString(), 
                    user.Username);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during sign in for user: {Email}", signInResource?.Email);
                return Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Signs up a new user.
        /// </summary>
        /// <param name="resource">The sign-up resource.</param>
        /// <returns>The authentication response.</returns>
        [HttpPost("sign-up")]
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerOperation(
            Summary = "Sign up",
            Description = "Sign up a user",
            OperationId = "SignUp")]
        [SwaggerResponse(StatusCodes.Status200OK, "The user was created", typeof(AuthResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The sign-up process has failed")]
        public async Task<IActionResult> SignUp([FromBody] SignUpResource resource)
        {
            if (resource == null)
            {
                _logger.LogWarning("SignUp: Request body is null");
                return BadRequest("Request body is required");
            }

            _logger.LogInformation("SignUp: Attempting to register user with email: {Email}", resource.Email);
            
            try
            {
                var existingUser = await _userQueryService.GetByEmailAsync(resource.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("SignUp failed: Email {Email} is already registered", resource.Email);
                    return BadRequest("Email is already registered");
                }

                _logger.LogDebug("Creating sign up command for email: {Email}", resource.Email);
                var signUpCommand = SignUpCommandFromResourceAssembler.ToCommandFromResource(resource);
                
                _logger.LogDebug("Processing sign up command");
                var user = await _userCommand.Handle(signUpCommand);
                
                if (user == null)
                {
                    _logger.LogWarning("Failed to create user with email: {Email}", resource.Email);
                    return Unauthorized("Failed to create user");
                }
                
                _logger.LogInformation("User created successfully. Generating JWT token for user ID: {UserId}", user.Id);
                var token = GenerateJwtToken(user);
                
                var response = AuthResponse.Create(
                    token,
                    user.Id,
                    user.Email.ToString(),
                    user.Username);
                
                _logger.LogInformation("User registration completed successfully for email: {Email}", resource.Email);
                return Ok(response);
            }
            catch (System.ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when processing sign up for email: {Email}", resource?.Email);
                return BadRequest(ex.Message);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error during sign up for user: {Email}", resource?.Email);
                return Unauthorized(ex.Message);
            }
        }
    }
}
