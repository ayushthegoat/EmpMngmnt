using Emp.Models;

namespace Emp.Repo
{
    public interface IEmployeeRepository
    {
       
        Task<Employee> GetByIdAsync(int id);

       
        
        Task UpdateAsync(Employee employee);
       
        Task<bool> EmployeeExistsAsync(int id);

        Task<Employee> GetByUserIdAsync(string userId);

    }

}
