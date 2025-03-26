using Microsoft.AspNetCore.Identity;

namespace MeSoftCase.WebApi.Domain.Entities;

public class AppUser : IdentityUser
{
    public string? ReferalCode { get; set; }
}