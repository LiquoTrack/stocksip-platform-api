namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;

/// <summary>
///    Record class that serves as a Value Object for the capacity of a warehouse.
/// </summary>
public record Capacity()
{
    /// <summary>
    ///     The value of the capacity.
    /// </summary>
    private decimal Value { get; } = 0;

    /// <summary>
    ///     Default constructor for Capacity.
    /// </summary>
    /// <param name="totalCapacity">
    ///     The total capacity value of the warehouse. Must be a non-negative value and less than or equal to 100,000.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     Thrown when the provided total capacity value is negative or exceeds 100,000.
    /// </exception>
    public Capacity(decimal totalCapacity) : this()
    {
        if (!IsCapacityValid(totalCapacity))
            throw new ArgumentException($"{nameof(totalCapacity)} must be greater than {nameof(Capacity)}");
        
        Value = totalCapacity;
    }

    /// <summary>
    ///     Method to validate the capacity value.
    /// </summary>
    /// <param name="totalCapacity">
    ///     The total capacity value of the warehouse to validate.
    /// </param>
    /// <returns>
    ///     A boolean indicating whether the capacity value is valid (non-negative and less than or equal to 100,000).
    /// </returns>
    private static bool IsCapacityValid(decimal totalCapacity) =>
        totalCapacity is >= 0 and <= 100000;
}