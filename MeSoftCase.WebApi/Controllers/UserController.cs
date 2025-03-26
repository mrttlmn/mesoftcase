using MediatR;
using MeSoftCase.WebApi.Application.UseCases.LoginCommand;
using MeSoftCase.WebApi.Application.UseCases.Queries;
using MeSoftCase.WebApi.Application.UseCases.RegisterUserCommand;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeSoftCase.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IMediator mediator) : ControllerBase
{
    [HttpPost("register/manager")]
    public async Task<IActionResult> RegisterManagerAsync([FromBody] RegisterManagerCommand command) => Ok(await mediator.Send(command));
    
    [HttpPost("register/user")]
    public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterUserCommand command) => Ok(await mediator.Send(command));
    
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginCommand command) => Ok(await mediator.Send(command));
    
    [HttpGet("getusers")]
    public async Task<IActionResult> GetUsersCountAsync([FromQuery] GetUsersQuery query) => Ok(await mediator.Send(query));
}