using Clubhouse.Api.Controllers.Base;
using Clubhouse.Business.Contracts.Requests;
using Clubhouse.Business.Models;
using Clubhouse.Business.Services.Interfaces;
using Clubhouse.Shared.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Clubhouse.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
//[Authorize(AuthenticationSchemes = $"{AuthScheme.Bearer}")]
public class UserController : AppControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> Login([FromQuery] LoginRequest request)
    {
        var result = await _userService.Login(request);
        return ToActionResult(result);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> Create([FromBody] CreateMemberRequest request)
    {
        var result = await _userService.CreateMemberAsync(request);
        return ToActionResult(result);
    }
}