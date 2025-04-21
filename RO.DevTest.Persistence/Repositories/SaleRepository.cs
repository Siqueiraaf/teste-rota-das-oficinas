using Microsoft.EntityFrameworkCore;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RO.DevTest.Persistence.Repositories;

/// <summary>
/// Implementation of the sale repository interface
/// </summary>
public class SaleRepository : BaseRepository<Sale>, ISaleRepository
{
    public SaleRepository(DefaultContext context) : base(context) { }

    /// <summary>
    /// Gets all sales within a specified date range
    /// </summary>
    public async Task<IReadOnlyList<Sale>> GetSalesByPeriodAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .ThenInclude(i => i.Product)
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
            .ToListAsync();
    }

    /// <summary>
    /// Gets the total revenue generated within a specified date range
    /// </summary>
    public async Task<decimal> GetTotalRevenueByPeriodAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
            .SumAsync(s => s.TotalAmount);
    }

    /// <summary>
    /// Gets revenue data for each product sold within a specified date range
    /// </summary>
    public async Task<IReadOnlyList<(
        Guid ProductId, 
        string ProductName, 
        int Quantity, 
        decimal Revenue
        )>> GetProductRevenueByPeriodAsync(DateTime startDate, DateTime endDate)
    {
        var result = await _context.SaleItems
            .Include(si => si.Sale)
            .Include(si => si.Product)
            .Where(si => si.Sale.SaleDate >= startDate && si.Sale.SaleDate <= endDate)
            .GroupBy(si => new { si.ProductId, si.Product.Name })
            .Select(g => new
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                Quantity = g.Sum(si => si.Quantity),
                Revenue = g.Sum(si => si.Subtotal)
            })
            .ToListAsync();

        return result.Select(r => (
        r.ProductId, 
        r.ProductName, 
        r.Quantity, 
        r.Revenue)).ToList();
    }
}
