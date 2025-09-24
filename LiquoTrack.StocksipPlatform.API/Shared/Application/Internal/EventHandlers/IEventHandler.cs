using Cortex.Mediator.Notifications;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Events;

namespace LiquoTrack.StocksipPlatform.API.Shared.Application.Internal.EventHandlers;

/// <summary>
///     This class serves as a base interface for all event handlers.
/// </summary>
/// <typeparam name="TEvent">
///     The type of event to handle.
/// </typeparam>
public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IEvent
{
    
}