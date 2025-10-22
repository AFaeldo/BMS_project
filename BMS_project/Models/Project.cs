using Microsoft.AspNetCore.Mvc;

namespace BaranggayManagementSystem.Models
{
    public class Project : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
