using Emp.Data;
using Emp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using NuGet.Versioning;

namespace Emp.Repo
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public EmployeeRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }



        public async Task<Employee> GetByIdAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task AddAsync(Employee employee)
        {
            //adding changes to the model database
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();




            //converting dateOnly to dateTime
            DateOnly dobDateOnly = employee.Dob;
            TimeOnly timeOfDay = TimeOnly.FromDateTime(DateTime.Now); 
            DateTime dobDateTime = dobDateOnly.ToDateTime(timeOfDay);


            var user = new ApplicationUser
            {
                UserName = employee.Email,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Name = employee.Name,
                Age = employee.Age,
                Dob = dobDateTime, 
                Address = employee.Address,
                IsAdmin = employee.IsAdmin
            };

            var defaultPass = "Default@123";
            var result = await _userManager.CreateAsync(user, defaultPass);

            if (result.Succeeded)
            {
                if (employee.IsAdmin)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "Employee");
                }
                return;
            }
            return;
        }

        public async Task UpdateAsync(Employee employee)
        {
            DateOnly dobDateOnly = employee.Dob;
            TimeOnly timeOfDay = TimeOnly.FromDateTime(DateTime.Now);
            DateTime dobDateTime = dobDateOnly.ToDateTime(timeOfDay);

            var existingEmployee = await _context.Employees
                .Where(e => e.Id == employee.Id)
                .FirstOrDefaultAsync();

            if (existingEmployee != null)
            {
                existingEmployee.Name = employee.Name;
                existingEmployee.Age = employee.Age;
                existingEmployee.Dob = employee.Dob;
                existingEmployee.Address = employee.Address;
                existingEmployee.PhoneNumber = employee.PhoneNumber;
                existingEmployee.Email = employee.Email;
                existingEmployee.IsAdmin = employee.IsAdmin;

                _context.Employees.Update(existingEmployee);
                await _context.SaveChangesAsync();

            }
            var user = await _context.ApplicationsUsers.Where(i => i.Id == employee.UserId).FirstOrDefaultAsync();        
                    if (user != null)
                {
                    user.Email = employee.Email;
                    user.NormalizedEmail = employee.Email.ToUpper();
                    user.UserName = employee.Email;
                    user.PhoneNumber = employee.PhoneNumber;
                    user.Name = employee.Name;
                    user.Address = employee.Address;
                    user.Dob = dobDateTime;
                    user.Age = employee.Age;
                    await _userManager.UpdateAsync(user);
                }
            }
        

        public async Task<bool> EmployeeExistsAsync(int id)
        {
            return await _context.Employees.AnyAsync(e => e.Id == id);
        }

        public async Task<Employee> GetByUserIdAsync(string userId)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
        }
    }
}
