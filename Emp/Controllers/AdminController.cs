using Emp.Models;
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
        public async Task Create([FromBody]AdminAuthModel adminModel)
        {

            await _adminRepository.Create(adminModel);
             
        }
     
    }
}
