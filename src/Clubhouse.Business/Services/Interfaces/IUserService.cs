using Clubhouse.Business.Contracts.Requests;
using Clubhouse.Business.Contracts.Responses;
using Clubhouse.Business.Models;
using Clubhouse.Shared.Contracts;

namespace Clubhouse.Business.Services.Interfaces;

public interface IUserService
{
    Task<IApiResponse<LoginResponse>> Login(LoginRequest request);

    Task<IApiResponse<bool>> CreateMemberAsync(CreateMemberRequest request,
        CancellationToken ct = default);
}