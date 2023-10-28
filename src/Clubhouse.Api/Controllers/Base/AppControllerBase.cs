using Clubhouse.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace Clubhouse.Api.Controllers.Base;
public abstract class AppControllerBase : ControllerBase
{
    public IActionResult ToActionResult<T>(IApiResponse<T> apiResponse)
    {
        return StatusCode(apiResponse.Code, apiResponse);
    }
}
