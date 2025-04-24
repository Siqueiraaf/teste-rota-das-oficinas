using System;
using System.Collections.Generic;

namespace RO.DevTest.Application.Features.Sale.Queries.GetSalesQuery;

public class SalesVm
{
    public List<SaleListDto> Sales { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}

public class SaleListDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime SaleDate { get; set; }
    public int ItemCount { get; set; }
}
