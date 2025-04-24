using MediatR;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Product.Queries.GetProductByIdQuery;
using System.Linq.Expressions;

namespace RO.DevTest.Application.Features.Product.Queries.GetProductsQuery;

/// <summary>
/// Handler for retrieving a paginated list of products with filtering and sorting
/// </summary>
public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, ProductsVm>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductsVm> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        // Create filter predicate if search term is provided
        Expression<Func<Domain.Entities.Product, bool>>? predicate = null;

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            predicate = p => p.Name.Contains(request.SearchTerm) || p.Description.Contains(request.SearchTerm);
        }

        // Create ordering function based on sort parameters
        Func<IQueryable<Domain.Entities.Product>, IOrderedQueryable<Domain.Entities.Product>>? orderBy = null;

        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            orderBy = request.SortBy.ToLower() switch
            {
                "name" => query => request.SortDesc ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "price" => query => request.SortDesc ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                "createdat" => query => request.SortDesc ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                _ => query => query.OrderBy(p => p.Name),
            };
        }
        else
        {
            orderBy = query => query.OrderBy(p => p.Name);
        }

        // Get total count of products matching the filter
        var totalCount = await _productRepository.CountAsync(predicate);

        // Get products with filtering and ordering
        var products = await _productRepository.GetAsync(
            predicate,
            orderBy,
            (string?)null, // forçar o tipo do argumento.
            true);

        // Apply pagination manually
        // In a real-world scenario, this would be done at the database level for better performance
        var paginatedProducts = products
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Calculate total pages
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        // Create and return the view model
        var result = new ProductsVm
        {
            Products = paginatedProducts.Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.StockQuantity,
                p.CreatedAt,
                p.UpdatedAt
            )).ToList(),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = totalPages,
            HasPreviousPage = request.PageNumber > 1,
            HasNextPage = request.PageNumber < totalPages
        };

        return result;
    }
}
