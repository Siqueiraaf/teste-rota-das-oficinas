using MediatR;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Contracts.Persistance.Repositories;

namespace RO.DevTest.Application.Features.Sale.Commands.DeleteSaleCommand
{
    public class DeleteSaleCommandHandler : IRequestHandler<DeleteSaleCommand, Unit>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IProductRepository _productRepository;
        private readonly IIdentityAbstractor _identityAbstractor;

        public DeleteSaleCommandHandler(
            ISaleRepository saleRepository,
            IProductRepository productRepository,
            IIdentityAbstractor identityAbstractor)
        {
            _saleRepository = saleRepository;
            _productRepository = productRepository;
            _identityAbstractor = identityAbstractor;
        }

        public async Task<Unit> Handle(
            DeleteSaleCommand request,
            CancellationToken cancellationToken)
        {
            // Buscar a venda
            var sale = await _saleRepository.GetByIdAsync(request.SaleId);
            if (sale == null)
            {
                throw new KeyNotFoundException($"Venda com ID {request.SaleId} não encontrada.");
            }

            // Verificar se o usuário existe
            var user = await _identityAbstractor.FindUserByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException($"Usuário com ID {request.UserId} não encontrado.");
            }

            // Verificar se o usuário é o dono da venda
            if (sale.UserId != request.UserId.ToString())
            {
                throw new UnauthorizedAccessException("Você não tem permissão para excluir esta venda.");
            }

            // Remover os itens da venda e atualizar o estoque dos produtos
            foreach (var item in sale.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Produto com ID {item.ProductId} não encontrado.");
                }

                // Restaurar a quantidade do produto no estoque
                product.StockQuantity += item.Quantity;
                await _productRepository.UpdateAsync(product);
            }

            // Excluir a venda
            await _saleRepository.DeleteAsync(sale);

            return Unit.Value;
        }

    }
}
