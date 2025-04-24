using MediatR;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Contracts.Persistance.Repositories;

namespace RO.DevTest.Application.Features.Sale.Queries.GetSaleByIdQuery;

public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, SaleDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IIdentityAbstractor _identityAbstractor;

    public GetSaleByIdQueryHandler(
        ISaleRepository saleRepository,
        IIdentityAbstractor identityAbstractor)
    {
        _saleRepository = saleRepository;
        _identityAbstractor = identityAbstractor;
    }

    public async Task<SaleDto> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.Id);

        if (sale == null)
        {
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found.");
        }

        var user = await _identityAbstractor.FindUserByIdAsync(sale.UserId);

        return new SaleDto
        {
            Id = sale.Id,
            UserId = sale.UserId,
            UserName = user?.Name ?? "Unknown User",
            TotalAmount = sale.TotalAmount,
            SaleDate = sale.SaleDate,
            Items = sale.Items.Select(i => new SaleItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "Unknown Product",
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Subtotal = i.Subtotal
            }).ToList()
        };
    }
}
