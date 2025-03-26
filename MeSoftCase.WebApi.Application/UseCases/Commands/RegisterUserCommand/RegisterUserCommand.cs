using FluentValidation;
using MediatR;
using MeSoftCase.WebApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace MeSoftCase.WebApi.Application.UseCases.RegisterUserCommand;

public record RegisterUserCommand(string UserName, string Password, string Email) : IRequest<Unit>;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
    }
}

public class RegisterUserCommandHandler(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager
    ) : IRequestHandler<RegisterUserCommand, Unit>
{
    public async Task<Unit> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userManager.FindByNameAsync(request.UserName);
        if (existingUser is not null) throw new InvalidOperationException("User already exists.");

        var userCreateResult = await userManager.CreateAsync(new AppUser()
                                                                    {
                                                                        UserName = request.UserName,
                                                                        Email = request.UserName,
                                                                    }, request.Password
                                                            );
        
        if (!userCreateResult.Succeeded) throw new ApplicationException(string.Join(',', userCreateResult.Errors));
        
        var roleResult = await roleManager.FindByNameAsync("Customer");
        if (roleResult is null)
        {
            var role = new IdentityRole("Customer");
            var createRoleResult = await roleManager.CreateAsync(role);
            if (!createRoleResult.Succeeded) 
                throw new ApplicationException(string.Join(',', createRoleResult.Errors.Select(e => e.Description).ToList()));
        }
        
        var createdUser = await userManager.FindByNameAsync(request.UserName);
        var addToRoleResult = await userManager.AddToRoleAsync(createdUser, "Customer");
        if (!addToRoleResult.Succeeded)
            throw new ApplicationException(string.Join(',', addToRoleResult.Errors.Select(e => e.Description).ToList()));
        
        return Unit.Value;
    }
}