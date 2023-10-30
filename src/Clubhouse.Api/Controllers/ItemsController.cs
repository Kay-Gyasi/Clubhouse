using Clubhouse.Api.Controllers.Base;
using Clubhouse.Business.Contracts.Requests;
using Clubhouse.Business.Models;
using Clubhouse.Business.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Clubhouse.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
//[Authorize(AuthenticationSchemes = $"{AuthScheme.Bearer}")]
public class ItemsController : AppControllerBase
{
    private readonly IItemService _itemService;

    public ItemsController(IItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> Add(AddItemRequest request)
    {
        var response = await _itemService.AddAsync(request);
        return ToActionResult(response);
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> Update(UpdateItemRequest request)
    {
        var response = await _itemService.UpdateAsync(request);
        return ToActionResult(response);
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> Update([FromQuery] string itemId)
    {
        var response = await _itemService.SoftDeleteAsync(itemId);
        return ToActionResult(response);
    }
}