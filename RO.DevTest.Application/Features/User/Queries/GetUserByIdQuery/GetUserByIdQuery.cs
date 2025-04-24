using MediatR;

namespace RO.DevTest.Application.Features.User.Queries.GetUserByIdQuery;

public record GetUserByIdQuery : IRequest<UserDto>
{
    public Guid Id { get; set; }
}
