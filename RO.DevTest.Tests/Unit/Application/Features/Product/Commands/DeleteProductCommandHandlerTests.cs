using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Product.Commands.DeleteProductCommand;
using Xunit;

namespace RO.DevTest.Tests.Unit.Application.Features.Product.Commands
{
    public class DeleteProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly DeleteProductCommandHandler _handler;

        public DeleteProductCommandHandlerTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _handler = new DeleteProductCommandHandler(_mockProductRepository.Object);
        }

        [Fact]
        public async Task Handle_ExistingProduct_ShouldReturnTrueAndDeleteProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Domain.Entities.Product
            {
                Id = productId,
                Name = "Test Product",
                Description = "Test Description",
                Price = 10.99m,
                StockQuantity = 100,
                CreatedAt = DateTime.UtcNow
            };

            var command = new DeleteProductCommand { Id = productId }; // Convertendo Guid para string

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync(product);

            _mockProductRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Domain.Entities.Product>()))
                .Returns(Task.CompletedTask); // Simulando a exclusão sem erro

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue(); // Esperamos que o comando retorne true
            _mockProductRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Domain.Entities.Product>()), Times.Once); // Verifica que o método DeleteAsync foi chamado uma vez
        }

        [Fact]
        public async Task Handle_NonExistingProduct_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new DeleteProductCommand { Id = productId }; // Convertendo Guid para string

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync((Domain.Entities.Product?)null); // Simulando que o produto não foi encontrado

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
