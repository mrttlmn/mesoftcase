using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MeSoftCase.UI.Filters;

public class AuthorizationFilter : IAuthorizationFilter
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationFilter(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var token = _httpContextAccessor.HttpContext.Request.Cookies["access_token"];
        if (string.IsNullOrEmpty(token))
        {
            context.Result = new RedirectToActionResult("Index","Home",null);
            return;
        }
    }
}