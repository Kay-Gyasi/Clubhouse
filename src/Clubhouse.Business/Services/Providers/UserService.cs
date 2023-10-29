using Akka.Util;
using Clubhouse.Business.Authentication;
using Clubhouse.Business.Contracts.Requests;
using Clubhouse.Business.Contracts.Responses;
using Clubhouse.Business.Models;
using Clubhouse.Business.Services.Interfaces;
using Clubhouse.Data;
using Clubhouse.Data.Entities;
using Clubhouse.Data.Repositories.Interfaces;
using Clubhouse.Shared.Contracts;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Clubhouse.Business.Services.Providers;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IJwtService _jwtService;

    public UserService(ILogger<UserService> logger,
        IUserRepository userRepository, IRoleRepository roleRepository,
        IJwtService jwtService)
    {
        _logger = logger;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _jwtService = jwtService;
    }

    public async Task<IApiResponse<LoginResponse>> Login(LoginRequest request)
    {
        var user = await _userRepository.Authenticate(request);
        if (user is null)
        {
            _logger.LogDebug("Log in credentials passed was invalid");
            return ApiResponse<LoginResponse>.Default.ToBadRequestApiResponse(
                "No user with specified credentials was found");
        }

        var authToken = await Task.Run(() => _jwtService.GetToken(user));
        return (new LoginResponse(authToken)).ToOkApiResponse();
    }

    public async Task<IApiResponse<bool>> CreateMemberAsync(CreateMemberRequest request,
        CancellationToken ct = default)
    {
        var userExists = await _userRepository.ExistsAsync(x 
            => x.Email == request.Email || x.PhoneNumber == request.PhoneNumber);
        if (userExists)
        {
            _logger.LogDebug("user already exists. {CreateMemberRequest}", request);
            return ApiResponse<bool>.Default.ToBadRequestApiResponse();
        }

        var user = new User
        {
            Username = request.Username,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
        };

        var roleName = request.Type switch
        {
            UserType.Manager => CommonConstants.UserRoles.Manager,
            UserType.BackOfficer => CommonConstants.UserRoles.BackOfficer,
            UserType.Customer => CommonConstants.UserRoles.Customer,
            _ => throw new ArgumentOutOfRangeException()
        };

        var role = await _roleRepository.GetAsync(x => x.Name == roleName, ct);
        if (role is null)
        {
            _logger.LogDebug("Role: {role} not found", roleName);
            return ApiResponse<bool>.Default.ToNotFoundApiResponse();
        }
        
        var memberRole = await _roleRepository.GetAsync(x => x.Name == CommonConstants.UserRoles.Member, ct);
        if (memberRole is null)
        {
            _logger.LogDebug("Member role not found");
            return ApiResponse<bool>.Default.ToNotFoundApiResponse();
        }

        user.AddToRoles(new Role[]{role, memberRole});

        var (passwordHash, passwordKey) = 
            await Task.Run(() => _userRepository.CreatePassword(request.Password), ct);
        user.Password = passwordHash;
        user.PasswordKey = passwordKey;

        var saved = await _userRepository.AddAsync(user, ct: ct);
        if (!saved)
        {
            _logger.LogDebug("Failed to save user in db");
            return false.ToFailedDependencyApiResponse();
        }

        return true.ToCreatedApiResponse();
    }
}