using System.Reflection;
using MeSoftCase.WebApi.Application.Common.Helpers;
using MeSoftCase.WebApi.Application.Interfaces;
using MeSoftCase.WebApi.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MeSoftCase.WebApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
        services.AddScoped<IJwtTokenHelper,JwtTokenHelper>();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        return services;
    }
}