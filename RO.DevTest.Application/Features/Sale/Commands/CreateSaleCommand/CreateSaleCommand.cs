using MediatR;
using System;
using System.Collections.Generic;

namespace RO.DevTest.Application.Features.Sale.Commands.CreateSaleCommand;

public class CreateSaleCommand : IRequest<CreateSaleResult>
{
    public string UserId { get; set; } = string.Empty;
    public List<SaleItemDto> Items { get; set; } = new List<SaleItemDto>();
}

public class SaleItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
