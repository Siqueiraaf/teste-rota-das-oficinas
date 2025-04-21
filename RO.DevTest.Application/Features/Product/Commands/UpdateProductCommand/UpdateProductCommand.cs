using MediatR;
using System;

namespace RO.DevTest.Application.Features.Product.Commands.UpdateProductCommand;

/// <summary>
/// Command for updating an existing product
/// </summary>
public class UpdateProductCommand : IRequest<UpdateProductResult>
{
    /// <summary>
    /// Unique identifier of the product to update
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Updated name of the product
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Updated description of the product
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Updated price of the product
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Updated stock quantity of the product
    /// </summary>
    public int StockQuantity { get; set; }
}
