using MediatR;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.Sale.Queries.GetSalesAnalysisQuery;

/// <summary>
/// Handler for retrieving sales analysis data for a specific period
/// </summary>
public class GetSalesAnalysisQueryHandler : IRequestHandler<GetSalesAnalysisQuery, SalesAnalysisVm>
{
    private readonly ISaleRepository _saleRepository;

    public GetSalesAnalysisQueryHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<SalesAnalysisVm> Handle(GetSalesAnalysisQuery request, CancellationToken cancellationToken)
    {
        // Get sales data for the specified period
        var sales = await _saleRepository.GetSalesByPeriodAsync(request.StartDate, request.EndDate);

        // Get total revenue for the period
        var totalRevenue = await _saleRepository.GetTotalRevenueByPeriodAsync(request.StartDate, request.EndDate);

        // Get product revenue breakdown
        var productRevenues = await _saleRepository.GetProductRevenueByPeriodAsync(request.StartDate, request.EndDate);

        // Create and return the view model
        var result = new SalesAnalysisVm
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalSales = sales.Count,
            TotalRevenue = totalRevenue,
            ProductRevenues = productRevenues.Select(pr => new ProductRevenueDto
            {
                ProductId = pr.ProductId,
                ProductName = pr.ProductName,
                Quantity = pr.Quantity,
                Revenue = pr.Revenue,
                PercentageOfTotalRevenue = totalRevenue > 0 ? (pr.Revenue / totalRevenue) * 100 : 0
            }).ToList()
        };

        return result;
    }
}
