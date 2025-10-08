using Cortex.Mediator;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using MongoDB.Driver;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.Persistence.MongoDB.Repositories;

/// <summary>
///     Repository implementation for Plans entity.
/// </summary>
public class PlanRepository(AppDbContext context, IMediator mediator) : BaseRepository<Plan>(context, mediator), IPlanRepository
{

    /// <summary>
    ///     The MongoDB collection for the Plan entity.
    /// </summary>
    private readonly IMongoCollection<Plan> _planCollection = context.GetCollection<Plan>();
    
    public async Task SeedPlansAsync()
    {
        var defaultPlans = new List<Plan>()
        {
            new Plan(
                EPlanType.Free,
                "Free plan with limited features",
                EPaymentFrequency.None,
                new Money(0m, new Currency("USD")),
                PlanLimits.For(EPlanType.Free)
            ),
            new Plan(
                EPlanType.Premium,
                "Premium plan with more features",
                EPaymentFrequency.Monthly,
                new Money(29.99m, new Currency("USD")),
                PlanLimits.For(EPlanType.Premium)
            ),
            new Plan(
                EPlanType.Enterprise,
                "Enterprise plan with unlimited features",
                EPaymentFrequency.Yearly,
                new Money(199.99m, new Currency("USD")),
                PlanLimits.For(EPlanType.Enterprise)
            )
        };
        
        var existingPlansTypes = await _planCollection
            .Find(FilterDefinition<Plan>.Empty)
            .Project(p => p.PlanType)
            .ToListAsync();
        
        var plansToAdd = defaultPlans
            .Where(plan => !existingPlansTypes.Contains(plan.PlanType))
            .ToList();
        
        if (plansToAdd.Count != 0) await _planCollection.InsertManyAsync(plansToAdd);
    }
}