using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;

/// <summary>
///     Static assembler class to convert RegisterWarehouseResource to RegisterWarehouseCommand.
/// </summary>
public static class RegisterWarehouseCommandFromResourceAssembler
{
    /// <summary>
    ///     Static method to convert RegisterWarehouseResource to RegisterWarehouseCommand.  
    /// </summary>
    /// <param name="resource">
    ///     The RegisterWarehouseResource to convert.
    /// </param>
    /// <returns>
    ///     A RegisterWarehouseCommand representation of the RegisterWarehouseResource.
    /// </returns>
    public static RegisterWarehouseCommand ToCommandFromResource(RegisterWarehouseResource resource)
    {
        // Create the address object
        var address = new WarehouseAddress(
            resource.AddressStreet, 
            resource.AddressCity, 
            resource.AddressDistrict, 
            resource.AddressPostalCode, 
            resource.AddressCountry);
        
        // Create the temperature limits object
        var tempLimits = new WarehouseTemperature(resource.TemperatureMin, resource.TemperatureMax);
        
        // Create the capacity object
        var capacity = new WarehouseCapacity(resource.Capacity);
        
        // Create the image url object
        var imageUrl = new ImageUrl(resource.ImageUrl);
        
        // Create the account id object
        var accountId = new AccountId(resource.AccountId);
        
        // Return the command
        return new RegisterWarehouseCommand(
                resource.Name,
                address,
                tempLimits,
                capacity,
                imageUrl,
                accountId
            );
    }
}