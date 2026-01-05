using Domain.Users;

namespace Application.Common.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid userId);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}
