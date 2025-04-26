using System;
using System.Collections.Generic;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Application.Features.Sale.Queries.GetSaleByIdQuery;

public class SaleDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime SaleDate { get; set; }
    public List<SaleItemDto> Items { get; set; } = [];
}

public class SaleItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public SaleStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}
