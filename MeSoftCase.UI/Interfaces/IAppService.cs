using MeSoftCase.UI.ResponseModels;

namespace MeSoftCase.UI.Interfaces;

public interface IAppService
{
    Task<string> CreateAccessTokenAsync(string username, string password,CancellationToken cancellationToken = default);

    Task<bool> RegisterManagerAsync(string username, string password, string referralCode, string email,
        CancellationToken cancellationToken = default);

    Task<bool> RegisterUserAsync(string username, string password, string email,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<GetUsersDataResponseModel>> GetUsersDataAsync(
        CancellationToken cancellationToken = default);

}