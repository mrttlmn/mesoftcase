using System.Diagnostics;
using MeSoftCase.UI.Filters;
using MeSoftCase.UI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeSoftCase.UI.Models;

namespace MeSoftCase.UI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAppService _appService;

    public HomeController(ILogger<HomeController> logger, IAppService appService)
    {
        _logger = logger;
        _appService = appService;
    }

    public IActionResult Index()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(string username, string password,CancellationToken cancellationToken = default)
    {
        var token = await _appService.CreateAccessTokenAsync(username, password,cancellationToken);
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login");
        }

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, 
            Secure = false, //development'ta olduğu için https zorunluluğunu kaldırdım, prodda xss'i önlemek için true olması gerekir.
            SameSite = SameSiteMode.Strict, 
            Expires = DateTime.Now.AddMinutes(59) 
        };

        Response.Cookies.Append("access_token", token, cookieOptions);
        return RedirectToAction("Dashboard", "Home"); 
    }
    
    [HttpPost]
    public async Task<IActionResult> Register(string username, string password,string email,string? refno,CancellationToken cancellationToken = default)
    {
        if (refno is null)
        {
            var isSuccess = await _appService.RegisterUserAsync(username,password,email,cancellationToken);
            if(isSuccess) return RedirectToAction("Index", "Home"); 
        }
        else
        {
            var isSuccess = await _appService.RegisterManagerAsync(username,password,refno,email,cancellationToken);
            if(isSuccess) return RedirectToAction("Index", "Home"); 
        }
        return RedirectToAction("Privacy", "Home"); 
    }

    public IActionResult Privacy()
    {
        return View();
    }
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken = default)
    {
        var data = await _appService.GetUsersDataAsync(cancellationToken);
        return View(data);
    }
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}