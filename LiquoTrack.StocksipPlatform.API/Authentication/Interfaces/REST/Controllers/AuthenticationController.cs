using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google.Requests;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Pipeline.Middleware.Attributes;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerTag("Authentication endpoints")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class AuthenticationController(
        IGoogleAuthService googleAuthService,
        IUserCommandService userCommandService,
        IUserQueryService userQueryService,
        ILogger<AuthenticationController> logger)
        : ControllerBase
    {
        private const int DefaultPageSize = 10;
        private const int MaxPageSize = 50;
        private const string GoogleAuthProvider = "Google";
        private const string LogPrefix = "[AuthenticationController]";

        private readonly IGoogleAuthService _googleAuthService = googleAuthService ?? throw new ArgumentNullException(nameof(googleAuthService));
        private readonly IUserCommandService _userCommandService = userCommandService ?? throw new ArgumentNullException(nameof(userCommandService));
        private readonly IUserQueryService _userQueryService = userQueryService ?? throw new ArgumentNullException(nameof(userQueryService));
        private readonly ILogger<AuthenticationController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        #region Google Authentication

        /// <summary>
        /// Authenticates a user using Google's OAuth 2.0 ID token
        /// </summary>
        /// <param name="request">Google authentication request containing ID token and client ID</param>
        /// <returns>Authentication response with user details and JWT token</returns>
        [HttpPost("auth/google")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
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
            _logger.LogInformation($"{LogPrefix} Starting Google Authentication");

            try
            {
                if (!ModelState.IsValid)
                {
                    return HandleInvalidModelState();
                }

                _logger.LogDebug($"{LogPrefix} Validating Google token for client: {request.ClientId}");
                var (user, token, error) = await _googleAuthService.AuthenticateWithGoogleAsync(request.IdToken, request.ClientId);

                if (error != null || user == null || token == null)
                {
                    _logger.LogWarning($"{LogPrefix} Google authentication failed: {error}");
                    return Unauthorized(new { error = error ?? "Authentication failed" });
                }

                _logger.LogInformation($"{LogPrefix} Successfully authenticated user: {user.Email}");
                return Ok(new { token, user });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{LogPrefix} Error during Google authentication");
                return HandleException(ex);
            }
        }

        #endregion

        #region Authentication

        /// <summary>
        /// Signs in a user with email and password
        /// </summary>
        /// <param name="signInResource">User credentials</param>
        [HttpPost("sign-in")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Sign in",
            Description = "Sign in with email and password"
        )]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SignIn([FromBody] SignInResource signInResource)
        {
            _logger.LogInformation($"{LogPrefix} Sign in attempt for user: {signInResource.Email}");

            try
            {
                var signInCommand = SignInCommandFromResourceAssembler.ToCommandFromResource(signInResource);
                var (user, token) = await _userCommandService.Handle(signInCommand);

                _logger.LogInformation($"{LogPrefix} Successful sign in for user: {user.Email}");
                var resource = AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(user, token);
                return Ok(resource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{LogPrefix} Sign in failed for user: {signInResource?.Email}");
                return Unauthorized(new
                {
                    error = "Authentication failed",
                    message = "Invalid email or password",
                    requestId = HttpContext.TraceIdentifier
                });
            }
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="signUpResource">User registration details</param>
        [AllowAnonymous]
        [HttpPost("sign-up")]
        [SwaggerOperation(
            Summary = "Sign up",
            Description = "Register a new user"
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SignUp([FromBody] SignUpResource signUpResource)
        {
            _logger.LogInformation($"{LogPrefix} New user registration: {signUpResource.Email}");

            try
            {
                var signUpCommand = SignUpCommandFromResourceAssembler.ToCommandFromResource(signUpResource);
                var result = await _userCommandService.Handle(signUpCommand);

                _logger.LogInformation($"{LogPrefix} User registered successfully: {signUpResource.Email}");
                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{LogPrefix} Registration failed for user: {signUpResource.Email}");
                return HandleException(ex);
            }
        }

        #endregion

        #region Private Helpers

        private void LogUserClaims()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                _logger.LogInformation($"{LogPrefix} Authenticated user: {User.Identity.Name}");
                _logger.LogDebug($"{LogPrefix} User claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"))}");
            }
        }

        private PaginatedResponse<UserListResponse> CreatePaginatedResponse(List<User> users, int page, int pageSize)
        {
            var totalCount = users.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var items = users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserListResponse
                {
                    Id = ParseUserId(u.Id.ToString()),
                    Email = u.Email?.ToString() ?? string.Empty,
                    EmailVerified = true,
                    Provider = GoogleAuthProvider,
                })
                .ToList();

            return new PaginatedResponse<UserListResponse>
            {
                Items = items,
                Page = page,
                PageSize = items.Count,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        private static int ParseUserId(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length < 8)
                return 0;

            try
            {
                return int.Parse(id.Substring(0, 8),
                    System.Globalization.NumberStyles.HexNumber);
            }
            catch
            {
                return 0;
            }
        }

        private IActionResult HandleInvalidModelState()
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning($"{LogPrefix} Invalid model state. Errors: {string.Join(", ", errors)}");

            return BadRequest(new
            {
                error = "Invalid request data",
                details = errors
            });
        }

        private IActionResult HandleException(Exception ex)
        {
            var errorId = Guid.NewGuid().ToString();
            _logger.LogError(ex, $"{LogPrefix} Error ID: {errorId}");

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                error = "An unexpected error occurred",
                errorId,
                details = ex.Message
            });
        }

        #endregion
    }
}