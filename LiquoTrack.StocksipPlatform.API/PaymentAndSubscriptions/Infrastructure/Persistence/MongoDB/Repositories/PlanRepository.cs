using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.Persistence.MongoDB.Repositories;

public class PlansRepository(AppDbContext context) : BaseRepository<Plan>(context), IPlanRepository
{
    public Task SeedPlansAsync()
    {
        throw new NotImplementedException();
    }
}