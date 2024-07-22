
using Emp.Data;
using Emp.Models;

using Emp.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NuGet.Packaging.Signing;
using System.Security.Claims;

namespace Emp.Controllers
{
    public class AdminController : Controller
    {

        private readonly IAdminRepository _adminRepository;

        private readonly ILogger<AdminController> _logger;
        private readonly IMemoryCache _cache;

        public AdminController(IAdminRepository adminRepository, ILogger<AdminController> logger, IMemoryCache cache)
        {
            _adminRepository = adminRepository;
            _logger = logger;
            _cache = cache;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminAuthModel adminModel)
        {
            if (ModelState.IsValid)
            {
                var isAuth = await _adminRepository.Create(adminModel);
                if (isAuth)
                {
                    _cache.Set("AdminAuth", "true", TimeSpan.FromMinutes(2));

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, mesage = "Invalid username or password" });
                }
            }
            else
            {
                return RedirectToAction("GeneralError", "Error");
            }

        }
        
        public async Task<IActionResult> Index()
        {
            var currUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currUserRole = await _adminRepository.GetUserRoleAsync(currUser);

            if(currUserRole != "Admin" || currUserRole != "SuperAdmin")
            {
                return RedirectToAction("AccessDenied", "Error");
            }
            return View(await _adminRepository.GetAllAsync());
        }


       
        public async Task<IActionResult> Edit(int id)
        {
            var currUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var admin = await _adminRepository.GetByIdAsync(id);

            if (admin == null)
            {
                return RedirectToAction("GeneralError", "Error");
            }

            var currUserRole = await _adminRepository.GetUserRoleAsync(currUserId);
            if(currUserRole == "Employee")
            {
                return RedirectToAction("AccessDenied", "Error");
            }
            var adminRole = await _adminRepository.GetUserRoleAsync(admin.UserId);
            
            if (currUserId == admin.UserId)
            {
                return View(admin);
            }
            if (currUserRole == "Admin" && adminRole == "Admin" && admin.UserId != currUserId)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            return View(admin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Name,Age,Dob,Address,PhoneNumber,Email,IsAdmin,UserId")] Employee admin)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                return View(admin);
            }



            try
            {
                await _adminRepository.UpdateAsync(admin);
                TempData["SuccessMessage"] = "Succesfully Edited";
                return RedirectToAction("Edit", new { id = admin.Id });

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _adminRepository.EmployeeExistsAsync(admin.Id))
                {
                    return RedirectToAction("GeneralError", "Error");
                }
                throw;
            }
        }


        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> CreateInside()
        {
           
            return View();
        }


        [HttpPost]
      
        public async Task<IActionResult> CreateInside([Bind("Id,Name,Age,Dob,Address,PhoneNumber,Email,IsAdmin,UserId")] Employee emp)
        {
            if (!ModelState.IsValid)
            {
                return View(emp);
            }
            var listOfEmp = await _adminRepository.GetAllAsync();


            var doesExist = listOfEmp.Any(e => e.Email.Equals(emp.Email, StringComparison.OrdinalIgnoreCase));

            if (doesExist)
            {

                return RedirectToAction("GeneralError", "Error");

            }
            else
            {
                try
                {

                    await _adminRepository.AddAsync(emp);

                    TempData["Success"] = "Employee Creation Succesfull";
                    return View("CreateInside");
                }
                catch (Exception e)
                {


                    ModelState.AddModelError(string.Empty, "An error occurred while creating the employee.");


                    return RedirectToAction("GeneralError", "Error");
                }
            }
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> DeleteCheck(int id)
        {
            var currUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currUserRole = await _adminRepository.GetUserRoleAsync(currUser);

            var userForDelete = await _adminRepository.GetByIdAsync(id);
            var currUserId = userForDelete.UserId;
            var userForDeleteRole = await _adminRepository.GetUserRoleAsync(currUserId);
            if (currUserRole == userForDeleteRole)
            {
                return RedirectToAction("AccessDenied", "Error");
            }
            if (currUserRole == "SuperAdmin" && userForDeleteRole == "Admin")
            {
                return View(userForDelete);
            }
            if ((currUserRole == "Admin" || currUserRole == "SuperAdmin") && userForDeleteRole == "Employee")
            {
                return View(userForDelete);

            }
            else
            {
                return RedirectToAction("GeneralError", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCheckConfirmed(int id)
        {
            var user = await _adminRepository.GetByIdAsync(id);
            await _adminRepository.DeleteAsync(id);

            TempData["Message"] = "User Deleted SuccesFully";
            return View("DeleteCheck");

        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> Details(int id)
        {
            var currUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currUserRole = await _adminRepository.GetUserRoleAsync(currUser);

            var userForDetails = await _adminRepository.GetByIdAsync(id);
            var userForDetailsRole = await _adminRepository.GetUserRoleAsync(userForDetails.UserId);

            if (currUser == userForDetails.UserId)
            {
                return View(userForDetails);
            }
            if (currUserRole == "Admin" && userForDetailsRole == "Admin")
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            if (currUserRole == "SuperAdmin" && userForDetailsRole == "Admin")
            {
                return View(userForDetails);
            }

            else if ((currUserRole == "Admin" || currUserRole == "SuperAdmin") && userForDetailsRole == "Employee")
            {
                return View(userForDetails);
            }
            else
            {
                return RedirectToAction("GeneralError", "Error");
            }

        }



    }
}