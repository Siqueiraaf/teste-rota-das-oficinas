using MediatR;
using System;

namespace RO.DevTest.Application.Features.Sale.Queries.GetSalesQuery;

public class GetSalesQuery : IRequest<SalesVm>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? SortBy { get; set; }
    public bool SortDesc { get; set; } = false;
}
