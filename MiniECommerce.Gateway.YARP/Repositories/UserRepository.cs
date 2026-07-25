using Microsoft.EntityFrameworkCore;
using MiniECommerce.Gateway.YARP.Context;
using MiniECommerce.Gateway.YARP.Models;

namespace MiniECommerce.Gateway.YARP.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> AnyByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(p => p.UserName == userName, cancellationToken);
    }
    public async Task<User?> GetByUsernameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(p => p.UserName == userName, cancellationToken);
    }
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
