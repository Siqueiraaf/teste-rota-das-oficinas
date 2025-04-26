using FluentValidation.Results;
using MediatR;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Exception;

namespace RO.DevTest.Application.Features.Sale.Commands.CreateSaleCommand;

public class CreateSaleCommandHandler(
    ISaleRepository saleRepository, 
    IProductRepository productRepository, 
    IIdentityAbstractor identityAbstractor) : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository = saleRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IIdentityAbstractor _identityAbstractor = identityAbstractor;

    public async Task<CreateSaleResult> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        // Validação com FluentValidation
        var validator = new CreateSaleCommandValidator();
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            throw new BadRequestException(validationResult);
        }

        var user = await _identityAbstractor.FindUserByIdAsync(request.UserId);
        if (user == null)
        {
            throw new KeyNotFoundException($"Usuário com ID {request.UserId} não encontrado.");
        }

        var sale = new Domain.Entities.Sale
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            SaleDate = DateTime.UtcNow,
            TotalAmount = 0
        };

        decimal totalAmount = 0;

        foreach (var item in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException($"Produto com ID {item.ProductId} não encontrado.");
            }

            if (product.StockQuantity < item.Quantity)
            {
                throw new BadRequestException($"Estoque insuficiente para o produto '{product.Name}'. " +
                    $"Disponível: {product.StockQuantity}, " +
                    $"Solicitado: {item.Quantity}");
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
                ProductName = i.Product?.Name ?? "Produto Desconhecido",
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Subtotal = i.Subtotal
            }).ToList()
        };
    }
}
