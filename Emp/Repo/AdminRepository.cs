using Emp.Controllers;
using Emp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Emp.Repo
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ILogger<AdminRepository> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        public AdminRepository(UserManager<IdentityUser> userManager, ILogger<AdminRepository> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task Create(AdminAuthModel model)
        {
            _logger.LogInformation("Admin Username: {Username}", model.Username);

            var user = await _userManager.FindByEmailAsync(model.Username);
            if (user != null)
            {
                var result = await _userManager.CheckPasswordAsync(user, model.Password);

                if (result)
                {
                    _logger.LogInformation("Password {Password}", model.Password);
                }
            }
            return;
        }
        
    }
}
