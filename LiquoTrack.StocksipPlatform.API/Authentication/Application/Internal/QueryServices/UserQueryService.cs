using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.QueryServices
{
    /// <summary>
    /// Query service for handling user-related operations.
    /// </summary>
    /// <remarks>
    /// This service is responsible for retrieving user entities from the database.
    /// It also handles the validation of authentication tokens for the user.
    /// </remarks>
    public class UserQueryService : IUserQueryService
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserQueryService"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        public UserQueryService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Handles the retrieval of a user by their account ID.
        /// </summary>
        /// <param name="query">The query containing the account ID.</param>
        /// <returns>The user entity if found; otherwise, null.</returns>
        public async Task<User?> Handle(GetAllUsersByAccountIdQuery query)
        {
            return await _userRepository.FindByIdAsync(query.AccountId.ToString());
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>The user entity if found; otherwise, null.</returns>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userRepository.FindByEmailAsync(email);
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>The user entity if found; otherwise, null.</returns>
        public async Task<IEnumerable<User>> GetUsersByEmailAsync(GetUserByEmailQuery query, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.FindByEmailAsync(query.Email);
            return user != null ? new List<User> { user } : new List<User>();
        }

        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of all users.</returns>
        public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetAllUsersAsync(cancellationToken);
        }
    }
}