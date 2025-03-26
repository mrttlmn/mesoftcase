namespace MeSoftCase.WebApi.Application.Interfaces;

public interface IJwtTokenHelper
{
    Task<string> GenerateJwtToken(string userName);
}