using MediatR;
using System;

namespace RO.DevTest.Application.Features.Product.Commands.CreateProductCommand;

/// <summary>
/// Command for creating a new product
/// </summary>
public class CreateProductCommand : IRequest<CreateProductResult>
{
    /// <summary>
    /// Name of the product
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the product
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Price of the product
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Initial quantity available in stock
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Maps command to domain entity
    /// </summary>
    public Domain.Entities.Product AssignTo()
    {
        return new Domain.Entities.Product
        {
            Id = Guid.NewGuid(),
            Name = Name,
            Description = Description,
            Price = Price,
            StockQuantity = StockQuantity,
            CreatedAt = DateTime.UtcNow
        };
    }
}
