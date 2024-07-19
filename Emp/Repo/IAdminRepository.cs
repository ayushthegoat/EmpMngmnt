using Emp.Models;

using Microsoft.AspNetCore.Mvc;

namespace Emp.Repo
{
    public interface IAdminRepository
    {
        Task<bool> Create(AdminAuthModel model);

        Task<IEnumerable<Employee>> GetAllAsync();

        Task<string> GetUserRoleAsync(string userId);
        Task UpdateAsync(Employee employee);
        Task<Employee> GetByIdAsync(int id);
        Task<bool> EmployeeExistsAsync(int id);
        Task<Employee> GetByUserIdAsync(string userId);

        Task AddAsync(Employee employee);
        Task DeleteAsync(int id);
    }
}