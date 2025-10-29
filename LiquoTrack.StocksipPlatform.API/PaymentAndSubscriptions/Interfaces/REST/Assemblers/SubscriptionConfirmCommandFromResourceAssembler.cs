using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Assemblers;

public class SubscriptionConfirmCommandFromResourceAssembler
{
    public static ConfirmPaymentCommand ToCommandFromResource(SubscriptionConfirmResource resource)
    {
        return new ConfirmPaymentCommand(
            resource.PreferenceId,
            resource.Status
        );
    }
}