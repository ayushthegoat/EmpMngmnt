
using Emp.Models;
using Emp.Models.Emp.Models;
using Emp.Repo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Emp.Controllers
{
    public class AdminController : Controller
    {

        private readonly IAdminRepository _adminRepository;

        public AdminController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminAuthModel adminModel)
        {

            var isAuth = await _adminRepository.Create(adminModel);
            if(isAuth)
            {
                HttpContext.Session.SetString("AdminAuth", "true");

                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, mesage = "Invalid username or password" });
            }
        }

    }
}