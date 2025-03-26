using FluentValidation;
using MediatR;
using MeSoftCase.WebApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MeSoftCase.WebApi.Application.UseCases.RegisterUserCommand;

public record RegisterManagerCommand(string UserName, string Password, string Email,string ReferalCode) : IRequest<Unit>;

public class RegisterManagerCommandValidator : AbstractValidator<RegisterManagerCommand>
{
    public RegisterManagerCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
           

        RuleFor(x => x.ReferalCode)
            .NotEmpty().WithMessage("Referal code is required.")
            .NotNull().WithMessage("Referal code is required.");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
    }
}

public class RegisterManagerCommandHandler(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager
    ) : IRequestHandler<RegisterManagerCommand, Unit>
{
    public async Task<Unit> Handle(RegisterManagerCommand request, CancellationToken cancellationToken)
    {
        var refererUser = await userManager
            .Users
            .Where(u => u.ReferalCode == request.ReferalCode)
            .FirstOrDefaultAsync(cancellationToken);
                          
        if (refererUser == null)
        {
            throw new InvalidOperationException($"User with referal code {request.ReferalCode} was not found.");
        }
        
        var existingUser = await userManager.FindByNameAsync(request.UserName);
        if (existingUser != null) throw new InvalidOperationException("User already exists.");

        var userCreateResult = await userManager.CreateAsync(new AppUser()
        {
            UserName = request.UserName,
            Email = request.UserName,
            ReferalCode = GenerateReferalToken()
        }, request.Password);

        if (!userCreateResult.Succeeded) 
            throw new ApplicationException(string.Join(',', userCreateResult.Errors.Select(e => e.Description)));

        var roleResult = await roleManager.FindByNameAsync("Manager");
        if (roleResult == null)
        {
            var role = new IdentityRole("Manager");
            var createRoleResult = await roleManager.CreateAsync(role);
            if (!createRoleResult.Succeeded) 
                throw new ApplicationException(string.Join(',', createRoleResult.Errors.Select(e => e.Description)));
        }
    
        var createdUser = await userManager.FindByNameAsync(request.UserName);
        var addToRoleResult = await userManager.AddToRoleAsync(createdUser, "Manager");
        if (!addToRoleResult.Succeeded)
            throw new ApplicationException(string.Join(',', addToRoleResult.Errors.Select(e => e.Description)));

        string GenerateReferalToken() => Guid.NewGuid().ToString();
        return Unit.Value;
    }

}