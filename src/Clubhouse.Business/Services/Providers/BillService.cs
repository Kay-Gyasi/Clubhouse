using Clubhouse.Business.Contracts.Requests;
using Clubhouse.Business.Models;
using Clubhouse.Business.Services.Interfaces;
using Clubhouse.Data.Entities;
using Clubhouse.Data.Repositories.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Clubhouse.Business.Services.Providers;

public class BillService : IBillService
{
    private readonly IBillRepository _billRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<BillService> _logger;

    public BillService(IBillRepository billRepository,
        IUserRepository userRepository, ILogger<BillService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _billRepository = billRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<IApiResponse<bool>> 
        AddEntriesAsync(AddBillEntryRequest request, CancellationToken ct = default)
    {
        try
        {
            var bill = await GetAsync(request.UserId);
            if (bill is null)
            {
                _logger.LogDebug("user not found");
                return false.ToBadRequestApiResponse("Invalid user");
            }

            var entries = request.Entries.Adapt<List<BillEntry>>();
            bill.AddEntries(entries);

            var saved = await _billRepository.UpdateAsync(bill, ct: ct);
            if (!saved)
            {
                _logger.LogDebug("failed to update bill");
                return false.ToFailedDependencyApiResponse();
            }

            return true.ToOkApiResponse();
        }
        catch (Exception a)
        {
            _logger.LogError(a, 
                "Error while adding bill entries for request. {BillEntryRequest}", request);
            return false.ToServerErrorApiResponse();
        }
    }

    public async Task<IApiResponse<bool>>
        RemoveEntryAsync(RemoveBillEntryRequest request, CancellationToken ct = default)
    {
        try
        {
            var bill = await GetAsync(request.UserId);
            if (bill is null)
            {
                _logger.LogDebug("user not found");
                return false.ToBadRequestApiResponse("Invalid user");
            }

            var entry = request.Entry.Adapt<BillEntry>();
            bill.RemoveEntry(entry);

            var saved = await _billRepository.UpdateAsync(bill, ct: ct);
            if (!saved)
            {
                _logger.LogDebug("failed to update bill");
                return false.ToFailedDependencyApiResponse();
            }

            return true.ToOkApiResponse();
        }
        catch (Exception a)
        {
            _logger.LogError(a, "Error while removing bill entry for request. {BillEntryRequest}", request);
            return false.ToServerErrorApiResponse();
        }
    }


    public async Task<IApiResponse<bool>> MakePaymentAsync(string billId)
    {
        //
        await Task.CompletedTask;

        return true.ToOkApiResponse();
    }

    private async Task<Bill?> GetAsync(string userId)
    {
        var bill = await _billRepository.GetAsync(x => x.UserId == userId);
        if (bill is not null) return bill;

        var user = await _userRepository.GetAsync(x => x.Id == userId);
        if (user is null)
        {
            _logger.LogError("userId does not exist");
            return null;
        }

        return new Bill
        {
            UserId = userId
        };
    }
}