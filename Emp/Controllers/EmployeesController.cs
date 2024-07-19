using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Emp.Data;
using Emp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using NuGet.Packaging.Signing;
using Emp.Repo;
using System.Security.Claims;

namespace Emp.Controllers
{
    public class EmployeesController : Controller
    {
     
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeesController> _logger;
       

       
        public EmployeesController(IEmployeeRepository employeeRepository, ILogger<EmployeesController> logger)
        {
          
            _employeeRepository = employeeRepository;
            _logger = logger;
           
        }

    

     
        
        [Authorize(Roles ="Admin, SuperAdmin, Employee")]
        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
       
            if (userId == null)
            {
                return Unauthorized();
            }
            var employee = await _employeeRepository.GetByUserIdAsync(userId);
            _logger.LogInformation("{userId}", employee.UserId);
            if (userId == null)
            {
                return NotFound();
            }

            return View(employee);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Name,Age,Dob,Address,PhoneNumber,Email,IsAdmin,UserId")] Employee employee)
        {
            _logger.LogInformation("{Id}", employee.Id);
            _logger.LogInformation("{userId}", employee.UserId);

            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            var existingEmployee = await _employeeRepository.GetByIdAsync(employee.Id);

            _logger.LogInformation("{userId}", existingEmployee.UserId);
            if (existingEmployee == null || existingEmployee.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            try
            {
                await _employeeRepository.UpdateAsync(employee);
                return RedirectToAction("OwnDetails", "Employees");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _employeeRepository.EmployeeExistsAsync(employee.Id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> OwnDetails()
        {
            // Retrieve the currently logged-in user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized(); // Return 401 Unauthorized if no user ID is found
            }

            // Get the employee details by user ID
            var employee = await _employeeRepository.GetByUserIdAsync(userId);
            if (employee == null)
            {
                return NotFound(); // Return 404 Not Found if no employee record is found
            }

            return View(employee); // Return the view with the employee details
        }
    }
}
