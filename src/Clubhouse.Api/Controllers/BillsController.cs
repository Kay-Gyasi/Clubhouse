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
public class BillsController : AppControllerBase
{
    private readonly IBillService _billService;

    public BillsController(IBillService billService)
    {
        _billService = billService;
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> AddEntries(AddBillEntryRequest request)
    {
        var response = await _billService.AddEntriesAsync(request);
        return ToActionResult(response);
    }
    
    [HttpPut("remove")]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> RemoveEntry(RemoveBillEntryRequest request)
    {
        var response = await _billService.RemoveEntryAsync(request);
        return ToActionResult(response);
    }
}