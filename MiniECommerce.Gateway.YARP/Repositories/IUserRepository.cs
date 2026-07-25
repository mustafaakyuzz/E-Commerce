using MiniECommerce.Gateway.YARP.Models;

namespace MiniECommerce.Gateway.YARP.Repositories;

public interface IUserRepository
{
    Task<bool> AnyByUserNameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string userName, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
