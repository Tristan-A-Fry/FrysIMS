using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using FrysIMS.API.Models;

public interface IJwtService
{
    string GenerateToken(string userId, string email);
}

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;

    public JwtService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

        public string GenerateToken(string userId, string email)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secret = jwtSettings["Secret"];
            //For Debugging
            // Console.WriteLine($"Secret Key (JwtService.cs) : {secret}");

            if (string.IsNullOrEmpty(secret))
            {
                throw new InvalidOperationException("JWT Secret is missing from appsettings.json");
            }

            var key = new SymmetricSecurityKey(Convert.FromBase64String(secret)); // ✅ Convert Secret to Bytes
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var user = _userManager.FindByIdAsync(userId).Result;
            var roles = _userManager.GetRolesAsync(user).Result;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId), 
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
              claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationInMinutes"] ?? "60")),
                signingCredentials: credentials
            );



            //For Debugging
            // Console.WriteLine($"✅ Generated Token: {new JwtSecurityTokenHandler().WriteToken(token)}");  // ✅ Debugging
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
}
