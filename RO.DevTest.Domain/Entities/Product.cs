using System;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Domain.Entities;

/// <summary>
/// Represents a product in the e-commerce system
/// </summary>
public class Product
{
    /// <summary>
    /// Unique identifier for the product
    /// </summary>
    public Guid Id { get; set; }

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
    /// Current quantity available in stock
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Date and time when the product was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the product was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    public ProductStatus Status { get; set; } = ProductStatus.Available;
}
