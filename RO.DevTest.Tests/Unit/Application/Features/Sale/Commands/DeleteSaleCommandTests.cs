using Moq;
using RO.DevTest.Application.Features.Sale.Commands.DeleteSaleCommand;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Tests.Unit.Application.Features.Sale.Commands
{
    public class DeleteSaleCommandHandlerTests
    {
        private readonly Mock<ISaleRepository> _mockSaleRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IIdentityAbstractor> _mockIdentityAbstractor;
        private readonly DeleteSaleCommandHandler _handler;

        public DeleteSaleCommandHandlerTests()
        {
            _mockSaleRepository = new Mock<ISaleRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockIdentityAbstractor = new Mock<IIdentityAbstractor>();
            _handler = new DeleteSaleCommandHandler(
                _mockSaleRepository.Object,
                _mockProductRepository.Object,
                _mockIdentityAbstractor.Object);
        }

        [Fact]
        public async Task Handle_ValidSale_ShouldDeleteSaleAndUpdateStock()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var saleId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var sale = new Domain.Entities.Sale
            {
                Id = saleId,
                UserId = userId.ToString(),
                Items = new List<SaleItem>
                {
                    new SaleItem { ProductId = productId, Quantity = 2 }
                }
            };

            var firstItem = sale.Items.First();

            // Comando para deletar a venda
            var command = new DeleteSaleCommand
            {
                SaleId = saleId,
                UserId = userId
            };

            _mockSaleRepository.Setup(repo => repo.GetByIdAsync(saleId)).ReturnsAsync(sale);

            _mockProductRepository.Setup(x => x.GetByIdAsync(firstItem.ProductId)).ReturnsAsync(new Domain.Entities.Product
            {
                Id = firstItem.ProductId,
                StockQuantity = 10
            });

            _mockProductRepository.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Product>())).Returns(Task.CompletedTask);

            _mockSaleRepository.Setup(x => x.DeleteAsync(sale)).Returns(Task.CompletedTask);

            _mockIdentityAbstractor.Setup(x => x.FindUserByIdAsync(userId.ToString()))
                .ReturnsAsync(new Domain.Entities.User { Id = userId.ToString() });

            // Act
            await _handler.Handle(command, default);

            // Assert
            _mockProductRepository.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.Product>()), Times.Once);
            _mockSaleRepository.Verify(x => x.DeleteAsync(sale), Times.Once);
        }

        [Fact]
        public async Task Handle_SaleNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var command = new DeleteSaleCommand
            {
                SaleId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            _mockSaleRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Domain.Entities.Sale?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_UserUnauthorized_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var saleId = Guid.NewGuid();
            var sale = new Domain.Entities.Sale
            {
                Id = saleId,
                UserId = "anotherUser",
                Items = new List<SaleItem>()
            };

            var command = new DeleteSaleCommand
            {
                SaleId = saleId,
                UserId = userId
            };

            _mockSaleRepository.Setup(repo => repo.GetByIdAsync(saleId)).ReturnsAsync(sale);
            _mockIdentityAbstractor.Setup(x => x.FindUserByIdAsync(userId.ToString()))
                .ReturnsAsync(new Domain.Entities.User { Id = userId.ToString() });

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, default));
        }
    }
}
