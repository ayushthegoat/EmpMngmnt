using Microsoft.AspNetCore.Mvc;

namespace Emp.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();

        }


        public IActionResult GeneralError()
        {
            return View();
        }

    }
}
