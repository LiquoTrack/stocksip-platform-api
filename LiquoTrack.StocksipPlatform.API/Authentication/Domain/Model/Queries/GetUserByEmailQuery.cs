using System;
using System.Collections.Generic;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Queries
{
    public class GetUserByEmailQuery
    {
        public string Email { get; }

        public GetUserByEmailQuery(string email)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }
    }
}
