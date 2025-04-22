using MediatR;
using System;

namespace RO.DevTest.Application.Features.Sale.Queries.GetSalesAnalysisQuery;

/// <summary>
/// Query for retrieving sales analysis data for a specific period
/// </summary>
public class GetSalesAnalysisQuery : IRequest<SalesAnalysisVm>
{
    /// <summary>
    /// Start date of the analysis period
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// End date of the analysis period
    /// </summary>
    public DateTime EndDate { get; set; }
}
