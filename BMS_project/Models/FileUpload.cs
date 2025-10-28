using Microsoft.AspNetCore.Mvc;

namespace BaranggayManagementSystem.Models
{
    public class FileUpload : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
