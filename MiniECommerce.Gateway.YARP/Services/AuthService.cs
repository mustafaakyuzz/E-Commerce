using MiniECommerce.Gateway.YARP.Dtos;
using MiniECommerce.Gateway.YARP.Models;
using MiniECommerce.Gateway.YARP.Repositories;
using TS.Result;

namespace MiniECommerce.Gateway.YARP.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtProvider _jwtProvider;

    public AuthService(IUserRepository userRepository, JwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<string>> RegisterAsync(RegisterDto request, CancellationToken cancellationToken = default)
    {
        bool isUserNameExist = await _userRepository.AnyByUserNameAsync(request.UserName, cancellationToken);
        if (isUserNameExist)
        {
            return Result<string>.Failure("This Username is already in use");
        }

        User user = new()
        {
            UserName = request.UserName,
            Password = request.Password,
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<string>.Succeed("User Registeration is successfull");
    }
    public async Task<Result<string>> LoginAsync(LoginDto request, CancellationToken cancellationToken = default)
    {
        User? user = await _userRepository.GetByUsernameAsync(request.UserName, cancellationToken);
        if(user is null)
        {
            return Result<string>.Failure("User cannot found");
        }
        string token = _jwtProvider.createToken(user);

        return Result<string>.Succeed(token);
    }
}
