using Clubhouse.Business.Contracts.Requests;
using Clubhouse.Business.Services.Interfaces;
using Clubhouse.Data;
using Clubhouse.Data.Entities;
using Clubhouse.Data.Repositories.Interfaces;

namespace Clubhouse.Business.Services.Providers;

public class InitializationService : IInitializationService
{
    private readonly IUserService _userService;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;

    public InitializationService(IUserService userService,
        IRoleRepository roleRepository,
        IUserRepository userRepository)
    {
        _userService = userService;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
    }

    public async Task InitializeDbAsync()
    {
        await SeedRoles();
        await SeedUsers();
    }

    private async Task SeedUsers()
    {
        if (await _userRepository.HasDataAsync()) return;
        var users = new CreateMemberRequest[]
        {
            new CreateMemberRequest()
            {
                Username = "Kay Gyasi",
                PhoneNumber = "0557833216",
                Email = "kofigyasidev@gmail.com",
                Password = "pass",
                Type = UserType.Manager
            }
        };

        foreach (var request in users)
        {
            await _userService.CreateMemberAsync(request);
        }
    }

    private async Task SeedRoles()
    {
        if (await _roleRepository.HasDataAsync()) return;
        var roles = new Role[]
        {
            new Role(CommonConstants.UserRoles.BackOfficer),
            new Role(CommonConstants.UserRoles.Manager),
            new Role(CommonConstants.UserRoles.Member),
            new Role(CommonConstants.UserRoles.Customer),
        };

        _ = await _roleRepository.AddAsync(roles);
    }
}