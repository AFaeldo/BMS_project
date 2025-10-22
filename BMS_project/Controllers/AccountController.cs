using Microsoft.AspNetCore.Mvc;

namespace BMS_project.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            ViewData["Title"] = "Login";
            return View();
        }

       
        public IActionResult Register()
        {
            ViewData["Title"] = "Register";
            return View();
        }

       
        public IActionResult ForgotPassword()
        {
            ViewData["Title"] = "Forgot Password";
            return View();
        }

      
        public IActionResult AccessDenied()
        {
            ViewData["Title"] = "Access Denied";
            return View();
        }

       
        [HttpPost]
        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }
    }
}
