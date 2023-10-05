using Microsoft.AspNetCore.Mvc;

namespace Hololive.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //login function
        public IActionResult AdminHome(string username, string password)
        {
            if (username == "admin" && password == "admin")
            {
                // Successful login
                return View(); ; // Redirect to AdminHome action
            }
            else
            {
                // Failed login
                ViewBag.ErrorMessage = "Invalid username or password";
                return View("Index"); // Replace "YourLoginView" with your actual login view name
            }
            //return View();
        }
    }
}
