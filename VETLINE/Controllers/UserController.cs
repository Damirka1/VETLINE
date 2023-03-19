using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VETLINE.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        // Only users with the "User" role can access this action
        public IActionResult Index()
        {
            return View();
        }
    }
}
