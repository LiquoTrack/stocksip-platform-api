using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using PayPal.Api;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;

/// <summary>
///     Aggregate entity representing a warehouse.
/// </summary>
public class Warehouse(
    string name,
    WarehouseAddress address,
    WarehouseTemperature temperature,
    WarehouseCapacity capacity,
    ImageUrl imageUrl,
    AccountId accountId
    ) : Entity
{
    /// <summary>
    ///     The name of the warehouse.
    /// </summary>
    public string Name { get; private set; } = ValidateName(name);

    /// <summary>
    ///     The address of the warehouse.
    /// </summary>
    public WarehouseAddress Address { get; private set; } = address;

    /// <summary>
    ///     The capacity of the warehouse.
    /// </summary>
    public WarehouseCapacity Capacity { get; private set; } = capacity;

    /// <summary>
    ///     The minimum and maximum temperature of the warehouse.
    /// </summary>
    public WarehouseTemperature Temperature { get; private set; } = temperature;

    /// <summary>
    ///     The image url of the warehouse.
    /// </summary>
    public ImageUrl ImageUrl { get; private set; } = imageUrl;

    /// <summary>
    ///     The identifier of the account that owns the warehouse.
    /// </summary>
    public AccountId AccountId { get; private set; } = accountId;

    /// <summary>
    ///     Command constructor to create a new Warehouse instance from a RegisterWarehouseCommand.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details to register a new warehouse.
    /// </param>
    /// <param name="imageUrl">
    ///     The image url of the warehouse.
    /// </param>
    public Warehouse(RegisterWarehouseCommand command, ImageUrl imageUrl) 
        : this(command.Name, command.Address, command.Temperature, command.Capacity, imageUrl, command.AccountId) 
    {}
    
    /// <summary>
    ///     This method validates the warehouse name.
    /// </summary>
    /// <exception cref="ArgumentException">
    ///     The name cannot be null or empty
    /// </exception>
    private static string ValidateName(string name) 
        => string.IsNullOrWhiteSpace(name) 
            ? throw new ArgumentException("The warehouse name cannot be null or empty.", nameof(name)) 
            : name;
    
    /// <summary>
    ///     Constructs a new instance of the Warehouse class using an UpdateWarehouseCommand.
    /// </summary>
    public void UpdateWarehouse(UpdateWarehouseInformationCommand command, ImageUrl imageUrl)
    {
        Name = ValidateName(command.Name);
        Address = command.NewAddress;
        Temperature = command.NewTempLimits;
        Capacity = command.TotalCapacity;
        ImageUrl = imageUrl;
    }
}