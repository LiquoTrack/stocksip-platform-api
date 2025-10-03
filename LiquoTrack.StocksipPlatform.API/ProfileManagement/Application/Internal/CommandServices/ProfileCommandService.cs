using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Services;

namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Application.CommandServices;

/// <summary>
/// Service implementation for handling profile commands.
/// </summary>
public class ProfileCommandService(IProfileRepository profileRepository) : IProfileCommandService
{
    /// <summary>
    /// Method to handle the registration of a new profile.
    /// </summary>
    /// <param name="command">
    /// The command containing the details for registering a new profile.
    /// </param>
    /// <returns>
    /// The newly registered profile.
    /// Or null if the profile could not be registered.
    /// </returns>
    public async Task<Profile?> Handle(CreateProfileCommand command)
    {
        // Verifies that the profile doesn't already exist for this user
        if (await profileRepository.ExistsByUserIdAsync(command.UserId))
        {
            throw new InvalidOperationException($"A profile already exists for user ID: {command.UserId}. Cannot create a new profile for the same user.");
        }

        // Creates the profile with the given details
        var profile = new Profile(command);

        // Tries to add the profile to the repository
        try
        {
            await profileRepository.AddAsync(profile);
        }
        // If the profile could not be added, throws an exception
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new InvalidOperationException($"Failed to create profile for user ID: {command.UserId}", e);
        }
        
        // Returns the newly created profile
        return profile;
    }

    /// <summary>
    /// Method to handle the update of profile information.
    /// </summary>
    /// <param name="command">
    /// The command containing the details for updating the profile information.
    /// </param>
    /// <returns>
    /// The updated profile.
    /// Or null if the profile could not be updated.
    /// </returns>
    public async Task<Profile?> Handle(UpdateProfileCommand command)
    {
        // Verifies that the profile exists
        var profileToUpdate = await profileRepository.FindByIdAsync(command.ProfileId)
                              ?? throw new InvalidOperationException($"Could not find the profile to update with identifier {command.ProfileId}.");
        
        // Updates the profile with the given details
        profileToUpdate.UpdateInformation(command);

        // Tries to update the profile in the repository
        try
        {
            await profileRepository.UpdateAsync(profileToUpdate);
        }
        // If the profile could not be updated, throws an exception
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new InvalidOperationException($"Failed to update profile with ID: {command.ProfileId}", e);
        }
        
        // Returns the updated profile
        return profileToUpdate;
    }

    /// <summary>
    /// Method to delete a profile.
    /// </summary>
    /// <param name="profileId">
    /// The ID of the profile to delete.
    /// </param>
    /// <returns>
    /// True if the profile was deleted successfully; otherwise, false.
    /// </returns>
    public async Task<bool> DeleteProfileAsync(string profileId)
    {
        // Verifies that the profile exists
        var profileToDelete = await profileRepository.FindByIdAsync(profileId);
        
        if (profileToDelete == null)
        {
            return false;
        }

        // Tries to delete the profile from the repository
        try
        {
            await profileRepository.DeleteAsync(profileToDelete);
        }
        // If the profile could not be deleted, throws an exception
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new InvalidOperationException($"Failed to delete profile with ID: {profileId}", e);
        }
        
        return true;
    }
}