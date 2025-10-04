using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Infrastructure.Persistence.MongoDB.Repositories;

/// <summary>
///     Repository implementation for the Profile aggregate. 
/// </summary>
public class ProfileRepository(AppDbContext context) : BaseRepository<Profile>(context), IProfileRepository
{
    /// <summary>
    ///     The MongoDB collection for the Profile aggregate.   
    /// </summary>
    private readonly IMongoCollection<Profile> _profileCollection = context.GetCollection<Profile>();

    /// <inheritdoc />
    public new async Task AddAsync(Profile profile)
    {
        await _profileCollection.InsertOneAsync(profile);
    }
    
    /// <inheritdoc />
    public async Task UpdateAsync(Profile profile)
    {
        await _profileCollection.ReplaceOneAsync(
            x => x.Id == profile.Id,
            profile,
            new ReplaceOptions { IsUpsert = false }
        );
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Profile profile)
    {
        await _profileCollection.DeleteOneAsync(x => x.Id == profile.Id);
    }

    /// <inheritdoc />
    public async Task<Profile?> FindByIdAsync(ObjectId id)
    {
        return await _profileCollection
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public new async Task<Profile?> FindByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return null;

        return await _profileCollection
            .Find(x => x.Id == objectId)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<Profile?> FindByUserIdAsync(string userId)
    {
        return await _profileCollection
            .Find(x => x.UserId == userId)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Profile>> FindByFullNameAsync(string fullName)
    {
        return await _profileCollection
            .Find(x => x.FullName == fullName)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Profile>> FindAllAsync()
    {
        return await _profileCollection
            .Find(FilterDefinition<Profile>.Empty)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByUserIdAsync(string userId)
    {
        return await _profileCollection
            .Find(x => x.UserId == userId)
            .AnyAsync();
    }
}