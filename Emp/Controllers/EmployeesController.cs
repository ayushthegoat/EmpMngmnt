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

namespace Emp.Controllers
{
    public class EmployeesController : Controller
    {
     
        private readonly IEmployeeRepository _employeeRepository;
       

       
        public EmployeesController(IEmployeeRepository employeeRepository)
        {
          
            _employeeRepository = employeeRepository;
           
        }

     

        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> Index()
        {


            return View(await _employeeRepository.GetAllAsync());
        }

     
``````````````

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

          
            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

      
        
        public IActionResult Create()
        {
            return View();
        }

       
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Age,Dob,Address,PhoneNumber,Email,IsAdmin")] Employee employee)
        {
            if (ModelState.IsValid)
            {
          
              await _employeeRepository.AddAsync(employee);
              
            }


            return View(employee);


        }

        
        [Authorize(Roles ="Admin, Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

         
            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Age,Dob,Address,PhoneNumber,Email,IsAdmin")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                 
                    
                    await _employeeRepository.UpdateAsync(employee);
                }
                catch (DbUpdateConcurrencyException)
                {
                
                    if (!await _employeeRepository.EmployeeExistsAsync(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

          
            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
           
          
          
            
               await _employeeRepository.DeleteAsync(id);
            

          
         
            return RedirectToAction(nameof(Index));
        }

       
        private async Task<bool> EmployeeExists(int id)
        {
           
           return await _employeeRepository.EmployeeExistsAsync(id);
        }
    }
}
