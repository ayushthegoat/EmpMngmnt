using Emp.Models;
using Emp.Models.Emp.Models;
using Microsoft.AspNetCore.Mvc;

namespace Emp.Repo
{
    public interface IAdminRepository
    {
        Task<bool> Create(AdminAuthModel model);

    }
}