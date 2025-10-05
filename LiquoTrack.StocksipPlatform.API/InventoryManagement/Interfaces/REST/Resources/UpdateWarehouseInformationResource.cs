namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

/// <summary>
///     Resource class for updating warehouse information.
/// </summary>
public record UpdateWarehouseInformationResource(
    string Name, 
    string AddressStreet,
    string AddressCity,
    string AddressDistrict,
    string AddressPostalCode,
    string AddressCountry,
    decimal TemperatureMin,
    decimal TemperatureMax,
    double Capacity,
    string ImageUrl
    );