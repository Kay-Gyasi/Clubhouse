using Clubhouse.Business.Contracts.Requests;
using Clubhouse.Business.Contracts.Responses;
using Clubhouse.Business.Models;

namespace Clubhouse.Business.Services.Interfaces;

public interface IItemService
{
    Task<IApiResponse<AddItemResponse>> AddAsync(AddItemRequest request,
        CancellationToken ct = default);

    Task<IApiResponse<UpdateItemResponse>> UpdateAsync(UpdateItemRequest request,
        CancellationToken ct = default);

    Task<IApiResponse<bool>> SoftDeleteAsync(string itemId);
}