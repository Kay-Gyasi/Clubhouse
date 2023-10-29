using Clubhouse.Business.Authentication;

namespace Clubhouse.Business.Contracts.Responses;

public record LoginResponse(AuthToken Token);