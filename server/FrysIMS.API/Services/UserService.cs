using Microsoft.AspNetCore.Identity;
using FrysIMS.API.Models;


public interface IUserService
{
    Task<bool> IsEmailTakenAsync(string email);
    Task<bool> RegisterUserAsync(string email, string password, string role);
    Task<string> LoginUserAsync(string email, string password);
}


public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly RoleManager<IdentityRole> _roleManager; 

    public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtService jwtService, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _roleManager = roleManager;
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user != null; // Return true if user exists
    }

    public async Task<bool> RegisterUserAsync(string email, string password, string role)
    {
        var user = new ApplicationUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);
        var allowedRoles = new[] {"User", "InventorySpecialist", "ProjectManager"};
        
        if(!allowedRoles.Contains(role))
        {
          Console.WriteLine("Invalid Role");
          // return BadRequest(new { error = "Invalid role. Must be User, ProjectManager, or InventorySpecialist." });
          return false;
        }

        if (!result.Succeeded){
          Console.WriteLine("User Create Failed");
          return false;
        }

        if(!string.IsNullOrWhiteSpace(role) && await _roleManager.RoleExistsAsync(role))
        {
          await _userManager.AddToRoleAsync(user, role);
          Console.WriteLine($"Assigned role {role} to {email}");
        }
        return true;
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
