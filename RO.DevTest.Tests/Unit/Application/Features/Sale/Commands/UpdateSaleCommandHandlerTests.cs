using Moq;
using RO.DevTest.Application.Features.Sale.Commands.UpdateSaleCommand;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Tests.Unit.Application.Features.Sale.Commands
{
    public class UpdateSaleCommandHandlerTests
    {
        private readonly Mock<ISaleRepository> _mockSaleRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IIdentityAbstractor> _mockIdentityAbstractor;
        private readonly UpdateSaleCommandHandler _handler;

        public UpdateSaleCommandHandlerTests()
        {
            _mockSaleRepository = new Mock<ISaleRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockIdentityAbstractor = new Mock<IIdentityAbstractor>();
            _handler = new UpdateSaleCommandHandler(
                _mockSaleRepository.Object,
                _mockProductRepository.Object,
                _mockIdentityAbstractor.Object);
        }

        [Fact]
        public async Task Handle_ValidSale_ShouldUpdateSaleAndReturnResult()
        {
            // Arrange
            var userId = "user123";
            var saleId = Guid.NewGuid();
            var sale = new Domain.Entities.Sale
            {
                Id = saleId,
                UserId = userId,
                TotalAmount = 100m,
                Items = new List<SaleItem>
                {
                    new SaleItem { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 50, Subtotal = 50 }
                }
            };

            var firstItem = sale.Items.First();

            var command = new UpdateSaleCommand
            {
                SaleId = saleId,
                UserId = userId,
                Items = new List<SaleItemDto>
                {
                    new SaleItemDto { ProductId = firstItem.ProductId, Quantity = 2 }
                },
                PaymentMethod = PaymentMethod.CreditCard
            };

            // Mock dependencies
            _mockSaleRepository.Setup(repo => repo.GetByIdAsync(saleId)).ReturnsAsync(sale);
            _mockIdentityAbstractor.Setup(x => x.FindUserByIdAsync(userId)).ReturnsAsync(new Domain.Entities.User { Id = userId });
            _mockProductRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Domain.Entities.Product
            {
                Id = firstItem.ProductId,
                Name = "Product 1",
                Price = 50,
                StockQuantity = 100
            });
            _mockProductRepository.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Product>())).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.Equal(saleId, result.Id);
            Assert.Equal(100m, result.TotalAmount);
            Assert.Single(result.Items);
            Assert.Equal(2, result.Items[0].Quantity);
            _mockProductRepository.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.Product>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidSale_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var userId = "user123";
            var saleId = Guid.NewGuid();
            var command = new UpdateSaleCommand
            {
                SaleId = saleId,
                UserId = userId,
                Items = new List<SaleItemDto>
                {
                    new SaleItemDto { ProductId = Guid.NewGuid(), Quantity = 2 }
                }
            };

            // Mocking repository to return null sale
            _mockSaleRepository.Setup(repo => repo.GetByIdAsync(saleId)).ReturnsAsync((Domain.Entities.Sale?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_ProductNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var userId = "user123";
            var saleId = Guid.NewGuid();
            var sale = new Domain.Entities.Sale
            {
                Id = saleId,
                UserId = userId,
                TotalAmount = 100m,
                Items = new List<SaleItem>
                {
                    new SaleItem { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 50, Subtotal = 50 }
                }
            };

            var firstItem = sale.Items.First();

            var command = new UpdateSaleCommand
            {
                SaleId = saleId,
                UserId = userId,
                Items = new List<SaleItemDto>
                {
                    new SaleItemDto { ProductId = firstItem.ProductId, Quantity = 2 }
                }
            };

            // Mocking repository to return a valid sale
            _mockSaleRepository.Setup(repo => repo.GetByIdAsync(saleId)).ReturnsAsync(sale);

            // Mockando que o usuário existe (correção aqui!)
            _mockIdentityAbstractor.Setup(x => x.FindUserByIdAsync(userId)).ReturnsAsync(new Domain.Entities.User { Id = userId });

            // Mockando que o produto não foi encontrado
            _mockProductRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Domain.Entities.Product?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, default));
        }
    }
}
