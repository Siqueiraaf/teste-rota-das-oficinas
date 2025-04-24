using System;

namespace RO.DevTest.Application.Features.Product.Queries.GetProductByIdQuery;

/// <summary>
/// Data transfer object for product information
/// </summary>
public record ProductDto(
    /// <summary>
    /// Unique identifier of the product
    /// </summary>
    Guid Id,

    /// <summary>
    /// Name of the product
    /// </summary>
    string Name,

    /// <summary>
    /// Detailed description of the product
    /// </summary>
    string Description,

    /// <summary>
    /// Price of the product
    /// </summary>
    decimal Price,

    /// <summary>
    /// Quantity available in stock
    /// </summary>
    int StockQuantity,

    /// <summary>
    /// Date and time when the product was created
    /// </summary>
    DateTime CreatedAt,

    /// <summary>
    /// Date and time when the product was last updated
    /// </summary>
    DateTime? UpdatedAt
);
