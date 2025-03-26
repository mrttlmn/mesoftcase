using System.Xml.Schema;
using AutoMapper;
using MediatR;
using MeSoftCase.WebApi.Application.Common.Mappings;
using MeSoftCase.WebApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MeSoftCase.WebApi.Application.UseCases.Queries;

public record GetUsersQuery() : IRequest<IEnumerable<GetUsersQueryResponse>>;

public class GetUsersQueryResponse()
{
    public string Role { get; set; }
    public int Count { get; set; }
}

public class GetUsersQueryHandler(
    UserManager<AppUser> userManager, 
    RoleManager<IdentityRole> roleManager
    ) : IRequestHandler<GetUsersQuery, IEnumerable<GetUsersQueryResponse>>
{
    public async Task<IEnumerable<GetUsersQueryResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
       
        var roleUserCount = new Dictionary<string, int>();
        var roles = await roleManager.Roles.ToListAsync();
        
        var response = new List<GetUsersQueryResponse>();
        foreach (var role in roles)
        {
            var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);
            response.Add(new(){Role = role.Name,Count = usersInRole.Count});
        }
        return response;
        
    }
}