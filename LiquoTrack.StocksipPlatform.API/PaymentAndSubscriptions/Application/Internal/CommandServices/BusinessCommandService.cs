using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.CommandServices;

/// <summary>
///     Method to handle the creation of a new business. 
/// </summary>
public class BusinessCommandService(IBusinessRepository businessRepository) : IBusinessCommandService
{
    /// <summary>
    ///     Method to handle the creation of a new business.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for creating a new business.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains the newly created business.
    /// </returns>
    public async Task<Business?> Handle(CreateBusinessCommand command)
    {
        var business = new Business(new BusinessName(command.BusinessName));
        await businessRepository.AddAsync(business);
        return business;
    }
}