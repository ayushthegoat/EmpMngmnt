using Emp.Models;
using Microsoft.AspNetCore.Mvc;

namespace Emp.Repo
{
    public interface IAdminRepository
    {
        Task Create(AdminAuthModel model);

    }
}
