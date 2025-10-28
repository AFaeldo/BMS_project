using Microsoft.AspNetCore.Mvc;

namespace BaranggayManagementSystem.Models
{
    public class Barangay : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
