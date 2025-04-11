using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using Swashbuckle.AspNetCore.SwaggerGen;
using FrysIMS.API.Data;
using FrysIMS.API.Models;

var builder = WebApplication.CreateBuilder(args);

//JWT Settings
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"];

//For debugging
// Console.WriteLine($"🔑 Secret Key (Program.cs): {secretKey}");  // ✅ Debugging

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "FrysIMS API", Version = "v1" });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "bearer"
        }
    };

    options.AddSecurityDefinition("bearer", jwtSecurityScheme);
    options.OperationFilter<AuthOperationFilter>();

});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("LocalConnection")));

// ✅ Add Identity system
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add Jwt services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero, 
        NameClaimType = "sub",
        RoleClaimType = ClaimTypes.Role


    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});


// ✅ Add authentication and authorization
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IStockService, StockService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FrysIMS API V1");
        options.RoutePrefix = string.Empty; // Serve Swagger at root (http://localhost:5000)
        options.ConfigObject.AdditionalItems["persistAuthorization"] = true;
    });
}


//Seed Roles 
async Task SeedRolesAsync(IServiceProvider services)
{
  var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
  string[] roles = { "Admin", "ProjectManager", "InventorySpecialist", "User"};
  foreach (var role in roles)
  {
    if (!await roleManager.RoleExistsAsync(role))
    {
      await roleManager.CreateAsync(new IdentityRole(role));
    }
  }
}

//One time admin assignment 
async Task SeedAdminAsync (IServiceProvider services)
{
  var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
  var adminEmail = "tristanfry1234@gmail.com";
  var adminUser = await userManager.FindByEmailAsync(adminEmail);
  if (adminUser != null && !(await userManager.IsInRoleAsync(adminUser, "Admin"))) 
  {
    await userManager.AddToRoleAsync(adminUser, "Admin");
    Console.WriteLine($"Admin role assigned to {adminEmail}");
  }
  else
  {
    Console.WriteLine($"Admin user not found or already has a role");
  }
}

//Apply seeding 
using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;
  await SeedRolesAsync(services);
  await SeedAdminAsync(services);
}

app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
