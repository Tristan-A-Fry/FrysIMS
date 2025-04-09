

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using FrysIMS.API.Models;


public interface IUserService
{
    Task<bool> IsEmailTakenAsync(string email);
    Task<bool> RegisterUserAsync(string email, string password);
    Task<string> LoginUserAsync(string email, string password);
}


public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;

    public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user != null; // Return true if user exists
    }

    public async Task<bool> RegisterUserAsync(string email, string password)
    {
        var user = new ApplicationUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);
        return result.Succeeded;
    }

    public async Task<string> LoginUserAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null){
            Console.WriteLine("Login Failed: User not found");
            return null;
        } 

        bool isPasswordVaild = await _userManager.CheckPasswordAsync(user, password);
        if(!isPasswordVaild){
            Console.WriteLine("Login Failed: Incorrect Password.");
            return null; 
        }

        Console.WriteLine($"User {user.Email} logged in successfully.");
        return _jwtService.GenerateToken(user.Id, user.Email);
    }
}
