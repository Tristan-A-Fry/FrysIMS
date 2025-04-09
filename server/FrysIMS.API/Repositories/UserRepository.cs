
using Microsoft.AspNetCore.Identity;
using FrysIMS.API.Models;

public interface IUserRepository
{
    Task<ApplicationUser> GetUserByEmailAsync(string email);
}

public class UserRepository : IUserRepository
{

    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task<ApplicationUser> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }
}
