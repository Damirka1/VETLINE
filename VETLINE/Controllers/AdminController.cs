using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VETLINE.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // Only users with the "Admin" role can access this action
        public IActionResult Index()
        {
            return View();
        }
    }
}
