using Clubhouse.Business.Contracts.Requests;
using Clubhouse.Business.Contracts.Responses;
using Clubhouse.Business.Models;
using Clubhouse.Business.Services.Interfaces;
using Clubhouse.Data.Entities;
using Clubhouse.Data.Repositories.Interfaces;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Clubhouse.Business.Services.Providers;

public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<ItemService> _logger;

    public ItemService(IItemRepository itemRepository, ILogger<ItemService> logger)
    {
        _itemRepository = itemRepository;
        _logger = logger;
    }

    public async Task<IApiResponse<AddItemResponse>> AddAsync(AddItemRequest request,
        CancellationToken ct = default)
    {
        var itemExists = await _itemRepository.ExistsAsync(x => x.Name == request.Name);
        if (itemExists)
        {
            _logger.LogDebug("item already exists in db");
            return ApiResponse<AddItemResponse>.Default.ToBadRequestApiResponse();
        }

        var item = request.Adapt<Item>();
        var saved = await _itemRepository.AddAsync(item, ct: ct);
        if (!saved)
        {
            _logger.LogDebug("failed to add item to db");
            return ApiResponse<AddItemResponse>.Default.ToFailedDependencyApiResponse();
        }

        var response = item.Adapt<AddItemResponse>();
        return response.ToCreatedApiResponse();
    }

    public async Task<IApiResponse<UpdateItemResponse>> UpdateAsync(UpdateItemRequest request,
        CancellationToken ct = default)
    {
        var item = await _itemRepository.GetAsync(x => x.Id == request.Id, ct);
        if (item is null)
        {
            _logger.LogDebug("item not found");
            return ApiResponse<UpdateItemResponse>.Default.ToNotFoundApiResponse();
        }

        if (!string.IsNullOrEmpty(request.Name)) item.Name = request.Name;
        if (request.Price is not null) item.Price = request.Price.Value;

        var saved = await _itemRepository.UpdateAsync(item, ct: ct);
        if (!saved)
        {
            _logger.LogDebug("failed to update item to db");
            return ApiResponse<UpdateItemResponse>.Default.ToFailedDependencyApiResponse();
        }

        var response = item.Adapt<UpdateItemResponse>();
        return response.ToCreatedApiResponse();
    }

    public async Task<IApiResponse<bool>> SoftDeleteAsync(string itemId)
    {
        var itemExists = await _itemRepository.ExistsAsync(x => x.Id == itemId);
        if (!itemExists)
        {
            _logger.LogDebug("item not found");
            return ApiResponse<bool>.Default.ToNotFoundApiResponse();
        }

        var deleted = await _itemRepository.SoftDeleteAndSaveChangesAsync(itemId);
        if (!deleted)
        {
            _logger.LogDebug("failed to soft-delete item. {ItemId}", itemId);
            return ApiResponse<bool>.Default.ToFailedDependencyApiResponse();
        }

        return true.ToOkApiResponse();
    }
}