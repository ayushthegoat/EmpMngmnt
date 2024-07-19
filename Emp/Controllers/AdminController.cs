
using Emp.Data;
using Emp.Models;

using Emp.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using System.Security.Claims;

namespace Emp.Controllers
{
    public class AdminController : Controller
    {

        private readonly IAdminRepository _adminRepository;
        private readonly IEmployeeRepository _employeeRepository;


        public AdminController(IAdminRepository adminRepository, IEmployeeRepository employeeRepository)
        {
            _adminRepository = adminRepository;
            _employeeRepository = employeeRepository;
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

        public async Task<IActionResult> Index()
        {
            return View(await _adminRepository.GetAllAsync());
        }


        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> Edit(int id)
        {
            var currUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var admin = await _adminRepository.GetByIdAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            var currUserRole = await _adminRepository.GetUserRoleAsync(currUserId);
            var adminRole = await _adminRepository.GetUserRoleAsync(admin.UserId);
            if (currUserRole == "Admin" && adminRole == "Admin" && admin.UserId != currUserId)
            {
                return Forbid(); 
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

            //var existingAdmin = await _adminRepository.GetByUserIdAsync(admin.UserId);

            //if (existingAdmin == null || existingAdmin.UserId != userId)
            //{
            //    return Forbid();
            //}

            try
            {
                await _adminRepository.UpdateAsync(admin);
                TempData["Status"] = "Success";
                return RedirectToAction("Index", "Admin");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _adminRepository.EmployeeExistsAsync(admin.Id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        public async Task<IActionResult>  Delete(int id)
        {
            var currUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userForDelete = await _adminRepository.GetByIdAsync(id);
            var userForDeleteRole = await _adminRepository.GetUserRoleAsync(userForDelete.UserId);

            var currUserRole = await _adminRepository.GetUserRoleAsync(currUser);

            if((currUserRole == "Admin" || currUserRole == "SuperAdmin") && userForDeleteRole == "Employee")
            {
                return View(userForDelete);
            }
            else if (userForDelete.UserId == currUser)
            {
                return RedirectToAction("AccessDenied", "Error");
            }else
            {
                return RedirectToAction("AccessDenied", "Error");
            }

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userForDelete = await _adminRepository.GetByIdAsync(id);
            var userForDeleteRole = await _adminRepository.GetUserRoleAsync(userForDelete.UserId);

            var currUserRole = await _adminRepository.GetUserRoleAsync(currUser);

            if ((currUserRole == "Admin" || currUserRole == "SuperAdmin") && userForDeleteRole == "Employee")
            {
                await _adminRepository.DeleteAsync(id);
                return RedirectToAction("Index", "Admin");
            }
            else if (userForDelete.UserId == currUser)
            {
                return RedirectToAction("AccessDenied", "ErrorController");
            }
            else
            {
                return RedirectToAction("AccessDenied", "ErrorController");
            }
        }



        public async Task<IActionResult> CreateInside()
        {
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> CreateInside([Bind("Id,Name,Age,Dob,Address,PhoneNumber,Email,IsAdmin,UserId")] Employee emp)
        {
            
            var listOfEmp = await _adminRepository.GetAllAsync();

            
            var doesExist = listOfEmp.Any(e => e.Email.Equals(emp.Email, StringComparison.OrdinalIgnoreCase));

            if (doesExist)
            {
               
                return RedirectToAction("General", "Error"); 
            }
            else
            {
                try
                {
                   
                    await _adminRepository.AddAsync(emp);

               
                    return RedirectToAction("Index", "Admin");
                }
                catch (Exception e)
                {
                  
                   
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the employee.");

                  
                    return View(emp);
                }
            }
        }






    }
}