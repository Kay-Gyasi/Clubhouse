using Clubhouse.Business.Contracts.Requests;
using Clubhouse.Business.Models;

namespace Clubhouse.Business.Services.Interfaces;

public interface IBillService
{
    Task<IApiResponse<bool>>
        AddEntriesAsync(AddBillEntryRequest request, CancellationToken ct = default);

    Task<IApiResponse<bool>> MakePaymentAsync(string billId);

    Task<IApiResponse<bool>>
        RemoveEntryAsync(RemoveBillEntryRequest request, CancellationToken ct = default);
}