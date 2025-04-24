using MediatR;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using System.Linq.Expressions;

namespace RO.DevTest.Application.Features.Sale.Queries.GetSalesQuery;

public class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, SalesVm>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IIdentityAbstractor _identityAbstractor;

    public GetSalesQueryHandler(
        ISaleRepository saleRepository,
        IIdentityAbstractor identityAbstractor)
    {
        _saleRepository = saleRepository;
        _identityAbstractor = identityAbstractor;
    }

    public async Task<SalesVm> Handle(GetSalesQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Domain.Entities.Sale, bool>>? predicate = null;

        if (!string.IsNullOrWhiteSpace(request.UserId))
        {
            predicate = s => s.UserId == request.UserId;
        }

        if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            var startDatePredicate = PredicateBuilder.Create<Domain.Entities.Sale>(s => s.SaleDate >= request.StartDate.Value);
            var endDatePredicate = PredicateBuilder.Create<Domain.Entities.Sale>(s => s.SaleDate <= request.EndDate.Value);

            if (predicate == null)
            {
                predicate = startDatePredicate.And(endDatePredicate);
            }
            else
            {
                predicate = predicate.And(startDatePredicate).And(endDatePredicate);
            }
        }
        else if (request.StartDate.HasValue)
        {
            var startDatePredicate = PredicateBuilder.Create<Domain.Entities.Sale>(s => s.SaleDate >= request.StartDate.Value);

            if (predicate == null)
            {
                predicate = startDatePredicate;
            }
            else
            {
                predicate = predicate.And(startDatePredicate);
            }
        }
        else if (request.EndDate.HasValue)
        {
            var endDatePredicate = PredicateBuilder.Create<Domain.Entities.Sale>(s => s.SaleDate <= request.EndDate.Value);

            if (predicate == null)
            {
                predicate = endDatePredicate;
            }
            else
            {
                predicate = predicate.And(endDatePredicate);
            }
        }

        Func<IQueryable<Domain.Entities.Sale>, IOrderedQueryable<Domain.Entities.Sale>>? orderBy = null;

        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            orderBy = request.SortBy.ToLower() switch
            {
                "date" => query => request.SortDesc ? query.OrderByDescending(s => s.SaleDate) : query.OrderBy(s => s.SaleDate),
                "amount" => query => request.SortDesc ? query.OrderByDescending(s => s.TotalAmount) : query.OrderBy(s => s.TotalAmount),
                _ => query => query.OrderByDescending(s => s.SaleDate),
            };
        }
        else
        {
            orderBy = query => query.OrderByDescending(s => s.SaleDate);
        }

        var totalCount = await _saleRepository.CountAsync(predicate);

        var sales = await _saleRepository.GetAsync(
            predicate,
            orderBy,
            "Items",
            true);

        // Manual pagination (in a real app, this would be done at the database level)
        var paginatedSales = sales
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var saleListDtos = new List<SaleListDto>();

        foreach (var sale in paginatedSales)
        {
            var user = await _identityAbstractor.FindUserByIdAsync(sale.UserId);

            saleListDtos.Add(new SaleListDto
            {
                Id = sale.Id,
                UserId = sale.UserId,
                UserName = user?.Name ?? "Unknown User",
                TotalAmount = sale.TotalAmount,
                SaleDate = sale.SaleDate,
                ItemCount = sale.Items.Count
            });
        }

        var result = new SalesVm
        {
            Sales = saleListDtos,
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

// Helper class for combining predicates
public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate) => predicate;

    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        var parameter = Expression.Parameter(typeof(T));
        var leftVisitor = new ReplaceExpressionVisitor(left.Parameters[0], parameter);
        var leftExpression = leftVisitor.Visit(left.Body);
        var rightVisitor = new ReplaceExpressionVisitor(right.Parameters[0], parameter);
        var rightExpression = rightVisitor.Visit(right.Body);
        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(leftExpression!, rightExpression!), parameter);
    }

    private class ReplaceExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression? Visit(Expression? node)
        {
            if (node == _oldValue)
                return _newValue;
            return base.Visit(node);
        }
    }
}
