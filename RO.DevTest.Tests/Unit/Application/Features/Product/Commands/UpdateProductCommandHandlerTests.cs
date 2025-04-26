using FluentAssertions;
using Moq;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Product.Commands.UpdateProductCommand;

namespace RO.DevTest.Tests.Unit.Application.Features.Product.Commands
{
    public class UpdateProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly UpdateProductCommandHandler _handler;

        public UpdateProductCommandHandlerTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _handler = new UpdateProductCommandHandler(_mockProductRepository.Object);
        }

        [Fact]
        public async Task HandleExistingProductShouldReturnUpdatedProductResult()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var oldDate = DateTime.UtcNow.AddMinutes(-1);

            var existingProduct = new Domain.Entities.Product
            {
                Id = productId,
                Name = "Old Product Name",
                Description = "Old Product Description",
                Price = 20.00m,
                StockQuantity = 100,
                CreatedAt = oldDate,
                UpdatedAt = oldDate
            };

            var updateCommand = new UpdateProductCommand
            {
                Id = productId,
                Name = "Updated Product Name",
                Description = "Updated Product Description",
                Price = 25.50m,
                StockQuantity = 200
            };

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            _mockProductRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.Product>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(updateCommand, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(productId.ToString());
            result.Name.Should().Be("Updated Product Name");
            result.Description.Should().Be("Updated Product Description");
            result.Price.Should().Be(25.50m);
            result.StockQuantity.Should().Be(200);
            result.UpdatedAt.Should().BeAfter(oldDate);

            _mockProductRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.Product>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistingProduct_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var updateCommand = new UpdateProductCommand
            {
                Id = productId,
                Name = "Updated Product Name",
                Description = "Updated Product Description",
                Price = 25.50m,
                StockQuantity = 200
            };

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync((Domain.Entities.Product?)null); // Simulando que o produto não foi encontrado

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(updateCommand, CancellationToken.None));
        }
    }
}
