using CleanArchitecture_2025.Application.Services;
using CleanArchitecture_2025.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TS.Result;

namespace CleanArchitecture_2025.Application.Auth;
public sealed record LoginCommand(
    string UserNameOrEmail,
    string Password) : IRequest<Result<LoginCommandResponse>>;

public sealed record LoginCommandResponse
{
    public string AccessToken { get; set; } = default!;
}

internal sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginCommandResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IJwtProvider _jwtProvider;
    public LoginCommandHandler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtProvider jwtProvider)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtProvider = jwtProvider;
    }
    public async Task<Result<LoginCommandResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        AppUser? user = await _userManager.Users.FirstOrDefaultAsync(p => p.Email == request.UserNameOrEmail || p.UserName == request.UserNameOrEmail, cancellationToken);
        if (user is null)
        {
            return Result<LoginCommandResponse>.Failure("Kullanıcı bulunamadı.");
        }

        SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);

        if (signInResult.IsLockedOut)
        {
            TimeSpan? timeSpan = user.LockoutEnd - DateTime.UtcNow;
            if (timeSpan is not null)
            {
                return (500, $"Şifrenizi 5 defa yanlış girdiğiniz için kullanıcı {Math.Ceiling(timeSpan.Value.TotalMinutes)} dakika süreyle bloke edilmiştir.");
            }
            else
            {
                return (500, "Kullanıcınız 5 kez yanlış şifre girdiği için 5 dakika süreyle bloke edilmiştir.");
            }
        }

        if (signInResult.IsNotAllowed)
        {
            return (500, "Mail adresi onaylı değil");
        }

        if (!signInResult.Succeeded)
        {
            return (500, "Şifreniz yanlış");
        }

        // token üret
        var token = await _jwtProvider.CreateTokenAsync(user, cancellationToken);

        var response = new LoginCommandResponse()
        {
            AccessToken = token
        };

        return response;
    }
}
