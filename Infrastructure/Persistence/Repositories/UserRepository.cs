using Application.Common.Interfaces.Repository;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(GymTrackerDbContext db) : IUserRepository
{
    private readonly GymTrackerDbContext _db = db;

    public async Task AddAsync(User user) => await _db.Users.AddAsync(user);

    public async Task<User?> GetByEmailAsync(string email) => await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByIdAsync(Guid userId) => await _db.Users.FindAsync(userId);

    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
}
