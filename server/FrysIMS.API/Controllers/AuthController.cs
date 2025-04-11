using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService; 

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }
    

    [Authorize(Roles="Admin")] 
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {

        if (string.IsNullOrEmpty(request.Email))
        {
            return BadRequest(new { error = "Email is required." });
        }

        // Check if the email is already in use
        var isEmailTaken = await _userService.IsEmailTakenAsync(request.Email);
        if (isEmailTaken)
        {
            return BadRequest(new { error = "Email is already taken." });
        }


        if (request.Password != request.ConfirmPassword)
        {
            return BadRequest("Passwords do not match.");
        }
        //
        // var result = await _userService.RegisterUserAsync(request.Email, request.Password);
        // return result ? Ok("User registered successfully.") : BadRequest("Registration failed.");
        var result = await _userService.RegisterUserAsync(request.Email, request.Password, request.Role);
    
        if (result) {
            return Ok(new { message = "User registered successfully." }); // Return as JSON
        } 
        else {
            return BadRequest(new { error = "Registration failed." });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous] //NO Authentication requried
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var token = await _userService.LoginUserAsync(request.Email, request.Password);
        return token != null ? Ok(new { Token = token }) : Unauthorized();
    }

    [Authorize(Roles="Admin")]
    [HttpGet("whoami")]
    public IActionResult WhoAmI()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var claims = identity.Claims.Select(c => new { c.Type, c.Value });
        return Ok(claims);
    }
}
