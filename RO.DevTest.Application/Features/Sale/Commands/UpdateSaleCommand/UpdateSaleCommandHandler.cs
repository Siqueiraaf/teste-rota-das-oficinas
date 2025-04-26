using FluentValidation.Results;
using MediatR;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Exception;

namespace RO.DevTest.Application.Features.Sale.Commands.UpdateSaleCommand
{
    public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IProductRepository _productRepository;
        private readonly IIdentityAbstractor _identityAbstractor;

        public UpdateSaleCommandHandler(ISaleRepository saleRepository, IProductRepository productRepository, IIdentityAbstractor identityAbstractor)
        {
            _saleRepository = saleRepository;
            _productRepository = productRepository;
            _identityAbstractor = identityAbstractor;
        }

        public async Task<UpdateSaleResult> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            // Validação com FluentValidation
            var validator = new UpdateSaleCommandValidator();
            ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult);
            }

            var sale = await _saleRepository.GetByIdAsync(request.SaleId);
            if (sale == null)
            {
                throw new KeyNotFoundException($"Venda com ID {request.SaleId} não encontrada.");
            }

            var user = await _identityAbstractor.FindUserByIdAsync(request.UserId);
            if (user == null || user.Id != sale.UserId)
            {
                throw new UnauthorizedAccessException($"Usuário não autorizado a atualizar esta venda.");
            }

            // Atualizando os itens
            decimal totalAmount = 0;
            var updatedItems = new List<SaleItem>();

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

                var saleItem = sale.Items.FirstOrDefault(i => i.ProductId == item.ProductId);

                if (saleItem != null)
                {
                    // Atualiza a quantidade e o subtotal
                    saleItem.Quantity = item.Quantity;
                    saleItem.Subtotal = subtotal;
                    saleItem.UnitPrice = product.Price;
                }
                else
                {
                    // Adiciona um novo item de venda
                    saleItem = new SaleItem
                    {
                        Id = Guid.NewGuid(),
                        SaleId = sale.Id,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        Subtotal = subtotal
                    };
                    sale.Items.Add(saleItem);
                }

                product.StockQuantity -= item.Quantity;
                await _productRepository.UpdateAsync(product);
                updatedItems.Add(saleItem);
            }

            // Atualiza o total da venda
            sale.TotalAmount = totalAmount;
            sale.PaymentMethod = request.PaymentMethod;

            await _saleRepository.UpdateAsync(sale);

            return new UpdateSaleResult
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
}
