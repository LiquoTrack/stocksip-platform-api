using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;

/// <summary>
///     Interface for handling product exit-related queries.
/// </summary>
public interface IProductExitQueryService
{
    /// <summary>
    ///     Handler for the retrieval of all product exits for a given account ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the account ID for which product exits are to be retrieved.
    /// </param>
    /// <returns>
    ///     A list of product exits for the specified account ID, if any; otherwise, an empty list.   
    /// </returns>
    Task<IEnumerable<ProductExit>> Handle(GetAllProductExitsByAccountIdQuery query);
    
    /// <summary>
    ///     Handler for the retrieval of all product exits for a given warehouse ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the warehouse ID for which product exits are to be retrieved.
    /// </param>
    /// <returns>
    ///     A list of product exits for the specified warehouse ID, if any; otherwise, an empty list.  
    /// </returns>
    Task<IEnumerable<ProductExit>> Handle(GetAllProductExitsByWarehouseIdQuery query);
}