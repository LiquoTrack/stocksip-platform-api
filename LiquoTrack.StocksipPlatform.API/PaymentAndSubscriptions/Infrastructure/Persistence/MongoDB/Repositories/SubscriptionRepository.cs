using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.Persistence.MongoDB.Repositories;

public class SubscriptionRepository(AppDbContext context) : BaseRepository<Subscription> (context), ISubscriptionRepository
{
    
}