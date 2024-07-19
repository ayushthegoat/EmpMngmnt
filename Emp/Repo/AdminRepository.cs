

using Emp.Controllers;
using Emp.Data;
using Emp.Models;
using Emp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Emp.Repo
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ILogger<AdminRepository> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminRepository(UserManager<IdentityUser> userManager, ILogger<AdminRepository> logger, IEmployeeRepository employeeRepsitory, ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _logger = logger;
            _employeeRepository = employeeRepsitory;
            _context = context;
            _roleManager = roleManager;
        }


        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<bool> Create(AdminAuthModel model)
        {
            _logger.LogInformation("Admin Username: {Username}", model.Username);

            var user = await _userManager.FindByEmailAsync(model.Username);
            if (user != null)
            {
                var result = await _userManager.CheckPasswordAsync(user, model.Password);

                if (result)
                {
                    _logger.LogInformation("Password {Password}", model.Password);
                    var isInRole = await _userManager.IsInRoleAsync(user, "SuperAdmin");
                    if (isInRole)
                    {
                        return true;
                    }

                }
            }
            return false;
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
                user.IsAdmin = employee.IsAdmin;
                await _userManager.UpdateAsync(user);
            }
        }




        public async Task<Employee> GetByIdAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task<Employee> GetByUserIdAsync(string userId)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
        }

        public async Task<bool> EmployeeExistsAsync(int id)
        {
            return await _context.Employees.AnyAsync(e => e.Id == id);
        }
        public async Task<string> GetUserRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null; // Or throw an exception based on your error handling strategy
            }

            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault(); // Return the first role if there are multiple
        }

        public async Task DeleteAsync(int id)
        {
            var currUser = await _context.Employees.FindAsync(id);
            if (currUser != null)
            {
                _context.Employees.Remove(currUser);
                await _context.SaveChangesAsync();
            }

            var identityUser = await _userManager.FindByIdAsync(currUser.UserId);
            if (identityUser != null)
            {
                await _userManager.DeleteAsync(identityUser);
            }
        }

        public async Task AddAsync(Employee employee)
        {
            DateOnly dobDateOnly = employee.Dob;
            TimeOnly timeOfDay = TimeOnly.FromDateTime(DateTime.Now);
            DateTime dobDateTime = dobDateOnly.ToDateTime(timeOfDay);


            var user = new ApplicationUser
            {
                UserName = employee.Email,
                Email = employee.Email,
                Name = employee.Name,
                Age = employee.Age,
                Dob = dobDateTime,
                Address = employee.Address,
                PhoneNumber = employee.PhoneNumber,
                IsAdmin = false,

            };



            var defaultPassword = "Default@123";
            var result = await _userManager.CreateAsync(user, defaultPassword);

           

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                var adding = await _userManager.AddToRoleAsync(user, "Employee");

                var fromIdentity = _context.ApplicationsUsers.Where(i => i.Email == employee.Email).FirstOrDefault();

                if(adding.Succeeded && fromIdentity != null)
                {
                    var emp = new Employee
                    {
                        Email = employee.Email,
                        Name = employee.Name,
                        Age = employee.Age,
                        Address = employee.Address,
                        UserId = fromIdentity.Id,
                        Dob = employee.Dob,
                        PhoneNumber = employee.PhoneNumber,
                        IsAdmin = false,
                        

                    };
                   _context.Employees.Add(emp);
                    await _context.SaveChangesAsync();
                }

            }



        }
    

            
        
    }
}