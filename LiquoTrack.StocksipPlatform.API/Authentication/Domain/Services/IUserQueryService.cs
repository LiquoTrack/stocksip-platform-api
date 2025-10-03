using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Queries;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services
{
    public interface IUserQueryService
    {
        Task<User?> Handle(GetAllUsersByAccountIdQuery query);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetUsersByEmailAsync(GetUserByEmailQuery query, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of all users.</returns>
        Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Method to handle the GetUserByIdQuery
        /// </summary>
        /// <param name="query">
        ///     The query object containing the user ID for which the user is to be retrieved.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        /// </returns>
        Task<User?> Handle(GetUserByIdQuery query);
    }
}
