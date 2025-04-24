using MediatR;

namespace RO.DevTest.Application.Features.User.Commands.DeleteUserCommand;

public class DeleteUserCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}