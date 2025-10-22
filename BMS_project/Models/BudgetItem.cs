using Microsoft.AspNetCore.Mvc;

namespace BaranggayManagementSystem.Models
{
    public class BudgetItem : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
