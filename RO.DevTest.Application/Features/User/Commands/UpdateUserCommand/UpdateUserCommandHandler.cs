using MediatR;
using RO.DevTest.Application.Contracts.Persistance.Repositories;

namespace RO.DevTest.Application.Features.User.Commands.UpdateUserCommand;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UpdateUserResult>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UpdateUserResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        // Find the User by ID
        var user = await _userRepository.GetByIdAsync(request.Id);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {request.Id} not found.");
        }

        // Update User properties
        user.Name = request.Name;
        user.Email = request.Email;
        user.PasswordHash = request.Password;

        // Save changes to database
        await _userRepository.UpdateAsync(user);

        // Return result
        return new UpdateUserResult
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Password = user.PasswordHash
        };
    }
}
