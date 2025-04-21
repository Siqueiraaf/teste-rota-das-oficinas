using System;
using System.Collections.Generic;

namespace RO.DevTest.Domain.Entities;

/// <summary>
/// Represents a sale transaction in the e-commerce system
/// </summary>
public class Sale
{
    /// <summary>
    /// Unique identifier for the sale
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID of the user who made the purchase
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Reference to the user who made the purchase
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Total amount of the sale
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Date and time when the sale was made
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Collection of items included in this sale
    /// </summary>
    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
}
