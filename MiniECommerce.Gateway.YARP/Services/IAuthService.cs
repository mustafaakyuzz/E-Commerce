using MiniECommerce.Gateway.YARP.Dtos;
using TS.Result;

namespace MiniECommerce.Gateway.YARP.Services;

public interface IAuthService
{
    Task<Result<string>> RegisterAsync(RegisterDto request,  CancellationToken cancellationToken = default);
    Task<Result<string>> LoginAsync(LoginDto request, CancellationToken cancellationToken = default);
}
