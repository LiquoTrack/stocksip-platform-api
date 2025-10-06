using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Interfaces.ACL;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Application.Internal.ACL;

/// <summary>
/// Facade for handling profile-related operations, exposed to other bounded contexts.
/// </summary>
public class ProfileContextFacade : IProfileContextFacade
{
    private readonly IProfileCommandService _profileCommandService;
    private readonly ILogger<ProfileContextFacade> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfileContextFacade"/> class.
    /// </summary>
    /// <param name="profileCommandService">The profile command service.</param>
    /// <param name="logger">The logger instance.</param>
    public ProfileContextFacade(
        IProfileCommandService profileCommandService,
        ILogger<ProfileContextFacade> logger)
    {
        _profileCommandService =
            profileCommandService ?? throw new ArgumentNullException(nameof(profileCommandService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> CreateProfileAsync(
        string userId,
        string firstName,
        string? lastName,
        string? phoneNumber,
        IFormFile? profilePicture,
        string assignedRole)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));

        try
        {
            _logger.LogInformation("Initiating profile creation for user ID: {UserId}", userId);

            var command = new CreateProfileCommand(
                new PersonName(firstName, lastName),
                new PersonContactNumber(phoneNumber),
                phoneNumber,
                profilePicture,
                userId,
                assignedRole
            );

            var profile = await _profileCommandService.Handle(command);

            if (profile == null)
            {
                _logger.LogError("Profile creation failed for user ID: {UserId}", userId);
                throw new InvalidOperationException("Profile creation failed.");
            }

            _logger.LogInformation("Successfully created profile with ID: {ProfileId}", profile.Id);
            return profile.Id.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating profile for user ID: {UserId}", userId);
            throw new InvalidOperationException("An error occurred while creating the profile.", ex);
        }
    }
}