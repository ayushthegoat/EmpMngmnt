using Emp.Data;
using Emp.Models;
using Emp.Repo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Configure connection string and DbContext
var connectionString = builder.Configuration.GetConnectionString("conn");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

builder.Services.AddScoped<IAdminRepository, AdminRepository>();

builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddDefaultTokenProviders()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        await SeedRolesAsync(roleManager, userManager);
    }
    catch (Exception ex)
    {
        // Log any errors
        Console.Error.WriteLine($"Error seeding roles: {ex.Message}");
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
{
    string[] roleNames = { "Admin", "Employee", "SuperAdmin" };

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    var superEmail = "superadmin@accops.com";
    var superPass = "Super@123";

    if(userManager.Users.All(u => u.UserName != superEmail))
    {
        var superAdmin = new IdentityUser
        {
            UserName = superEmail,
            Email = superEmail,
            EmailConfirmed = true,
            TwoFactorEnabled = true
        };
        var result = await userManager.CreateAsync(superAdmin, superPass);
        if(result.Succeeded)
        {
            await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
        }
        else
        {
            throw new Exception("Error in Creating Super Admin");
        }
    }
}
