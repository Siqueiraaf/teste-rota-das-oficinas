using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Application.Contracts.Persistance.Repositories;

/// <summary>
/// Interface for Sale repository operations
/// </summary>
public interface ISaleRepository : IBaseRepository<Sale>
{
    /// <summary>
    /// Gets all sales within a specified date range
    /// </summary>
    /// <param name="startDate">Start date of the period</param>
    /// <param name="endDate">End date of the period</param>
    /// <returns>List of sales within the specified period</returns>
    Task<IReadOnlyList<Sale>> GetSalesByPeriodAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets the total revenue generated within a specified date range
    /// </summary>
    /// <param name="startDate">Start date of the period</param>
    /// <param name="endDate">End date of the period</param>
    /// <returns>Total revenue amount</returns>
    Task<decimal> GetTotalRevenueByPeriodAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets revenue data for each product sold within a specified date range
    /// </summary>
    /// <param name="startDate">Start date of the period</param>
    /// <param name="endDate">End date of the period</param>
    /// <returns>List of tuples containing product ID, name, quantity sold, and revenue</returns>
    Task<IReadOnlyList<(
        Guid ProductId, 
        string ProductName, 
        int Quantity, 
        decimal Revenue
        )>> GetProductRevenueByPeriodAsync(DateTime startDate, DateTime endDate);
}
