using System.Runtime.CompilerServices;
using FluentValidation;
using MediatR;
using MeSoftCase.WebApi.Application.Interfaces;
using MeSoftCase.WebApi.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace MeSoftCase.WebApi.Application.UseCases.LoginCommand;

public record LoginCommand(string UserName, string Password) : IRequest<LoginCommandResponse>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("UserName is required")
            .NotNull().WithMessage("UserName is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .NotNull().WithMessage("Password is required");
    }
}

public record LoginCommandResponse(string accessToken);

public class LoginCommandHandler(
    UserManager<AppUser> userManager,
    IHttpContextAccessor httpContextAccessor,
    IJwtTokenHelper jwtTokenHelper,
    IMemoryCache cache) : IRequestHandler<LoginCommand, LoginCommandResponse>
{
    public async Task<LoginCommandResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine("LoginCommandHandler.Handle");
        var ipAddress = GetClientIpAddress();
        if (cache.TryGetValue($"FailedAttempts_{ipAddress}", out int failedAttempts) && failedAttempts >= 10)
            throw new Exception("You are not able to login for a while");

        var user = await userManager.FindByNameAsync(request.UserName);
        if (user == null) throw new ApplicationException("User not found");
        
        var isPasswordCorrect = await userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordCorrect)
        {
            cache.Set($"FailedAttempts_{ipAddress}", ++failedAttempts, TimeSpan.FromMinutes(15));
            throw new Exception("Wrong password");
        }

        cache.Remove($"FailedAttempts_{ipAddress}");

        string GetClientIpAddress()
        {
            var ipAddress = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            return ipAddress ?? throw new ApplicationException("Remote Ip Address is null");
        }

        var token = await jwtTokenHelper.GenerateJwtToken(user.UserName);

        return new LoginCommandResponse(token);
    }
}