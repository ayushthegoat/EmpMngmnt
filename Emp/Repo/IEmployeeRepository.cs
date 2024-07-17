using Emp.Models;

namespace Emp.Repo
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee> GetByIdAsync(int id);
        Task AddAsync(Employee employee);
        Task UpdateAsync(Employee employee, string previousEmail);
        Task DeleteAsync(int id, Employee employee);
        Task<bool> EmployeeExistsAsync(int id);
    }

}
