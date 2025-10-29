using System.ComponentModel.DataAnnotations;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources
{
    /// <summary>
    /// Request model for updating an order's status
    /// </summary>
    public class UpdateOrderStatusRequest
    {
        /// <summary>
        /// The new status to set for the order
        /// </summary>
        [Required]
        public ESalesOrderStatuses NewStatus { get; set; }
        
        /// <summary>
        /// Optional reason for the status change
        /// </summary>
        public string Reason { get; set; }
    }
}
