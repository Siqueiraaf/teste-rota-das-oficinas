using MediatR;
using System;

namespace RO.DevTest.Application.Features.Sale.Queries.GetSaleByIdQuery;

public class GetSaleByIdQuery : IRequest<SaleDto>
{
    public Guid Id { get; set; }
}
