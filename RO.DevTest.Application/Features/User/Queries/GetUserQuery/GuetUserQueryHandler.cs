using System.Linq.Expressions;
using MediatR;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Contracts.Persistance.Repositories;


namespace RO.DevTest.Application.Features.User.Queries.GetUserQuery;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserVm>
{
    private readonly IUserRepository _userRepository;
    private readonly IIdentityAbstractor _identityAbstractor;

    public GetUserQueryHandler(
        IUserRepository userRepository,
        IIdentityAbstractor identityAbstractor)
    {
        _userRepository = userRepository;
        _identityAbstractor = identityAbstractor;
    }

    public async Task<UserVm> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Domain.Entities.User, bool>>? predicate = null;

        if (!string.IsNullOrWhiteSpace(request.UserId))
        {
            predicate = u => u.Id == request.UserId;
        }

        Func<IQueryable<Domain.Entities.User>, IOrderedQueryable<Domain.Entities.User>>? orderBy = null;

        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            orderBy = request.SortBy.ToLower() switch
            {
                "name" => query => request.SortDesc ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name),
                "email" => query => request.SortDesc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                _ => query => query.OrderBy(u => u.Name)
            };
        }
        else
        {
            orderBy = query => query.OrderBy(u => u.Name);
        }

        var totalCount = await _userRepository.CountAsync(predicate);

        var users = await _userRepository.GetAsync(
            predicate,
            orderBy,
            (List<Expression<Func<Domain.Entities.User, object>>>?)null,
            true);

        var paginatedUsers = users
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var userListDtos = paginatedUsers.Select(u => new UserListDto
        {
            Id = Guid.TryParse(u.Id, out var parsedId) ? parsedId : Guid.Empty,
            Name = u.Name ?? "Nome não informado",
            Email = u.Email ?? "Email não informado"
        }).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new UserVm
        {
            Users = userListDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = totalPages,
            HasPreviousPage = request.PageNumber > 1,
            HasNextPage = request.PageNumber < totalPages
        };
    }

}
