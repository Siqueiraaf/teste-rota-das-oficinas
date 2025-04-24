using Microsoft.EntityFrameworkCore;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Persistence.Repositories;

public class UserRepository(DefaultContext context)
    : BaseRepository<User>(context), IUserRepository 
{
    public async Task<User> GetByIdAsync(string id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id.ToString() == id)
            ?? throw new KeyNotFoundException($"User with ID {id} not found.");
    }
}
