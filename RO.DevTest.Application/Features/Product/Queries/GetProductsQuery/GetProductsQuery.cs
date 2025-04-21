using MediatR;

namespace RO.DevTest.Application.Features.Product.Queries.GetProductsQuery;

/// <summary>
/// Query for retrieving a paginated list of products with filtering and sorting
/// </summary>
public class GetProductsQuery : IRequest<ProductsVm>
{
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Optional search term to filter products by name or description
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Optional field name to sort by (e.g., "name", "price", "createdat")
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Whether to sort in descending order
    /// </summary>
    public bool SortDesc { get; set; } = false;
}
