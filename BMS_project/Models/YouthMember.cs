using Microsoft.AspNetCore.Mvc;

namespace BaranggayManagementSystem.Models
{
    public class YouthMember : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
