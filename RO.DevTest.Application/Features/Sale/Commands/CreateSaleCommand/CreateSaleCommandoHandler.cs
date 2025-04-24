using MediatR;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using FluentValidation;

namespace RO.DevTest.Application.Features.Sale.Commands.CreateSaleCommand;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IIdentityAbstractor _identityAbstractor;

    public CreateSaleCommandHandler(
        ISaleRepository saleRepository,
        IProductRepository productRepository,
        IIdentityAbstractor identityAbstractor)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _identityAbstractor = identityAbstractor;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityAbstractor.FindUserByIdAsync(request.UserId);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {request.UserId} not found.");
        }

        if (request.Items.Count == 0)
        {
            throw new ValidationException("Sale must have at least one item.");
        }

        var sale = new Domain.Entities.Sale
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            SaleDate = DateTime.UtcNow,
            TotalAmount = 0 // Will be calculatedS
        };

        decimal totalAmount = 0;

        foreach (var item in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {item.ProductId} not found.");
            }

            if (product.StockQuantity < item.Quantity)
            {
                throw new ValidationException($"Not enough stock for product {product.Name}. Available: {product.StockQuantity}, Requested: {item.Quantity}");
            }

            var subtotal = product.Price * item.Quantity;
            totalAmount += subtotal;

            var saleItem = new SaleItem
            {
                Id = Guid.NewGuid(),
                SaleId = sale.Id,
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price,
                Subtotal = subtotal
            };

            sale.Items.Add(saleItem);

            // Update product stock
            product.StockQuantity -= item.Quantity;
            await _productRepository.UpdateAsync(product);
        }

        sale.TotalAmount = totalAmount;

        await _saleRepository.AddAsync(sale);

        return new CreateSaleResult
        {
            Id = sale.Id,
            UserId = sale.UserId,
            TotalAmount = sale.TotalAmount,
            SaleDate = sale.SaleDate,
            Items = sale.Items.Select(i => new SaleItemResult
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
