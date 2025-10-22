using System.Diagnostics;
using BMS_project.Models;
using Microsoft.AspNetCore.Mvc;

namespace BMS_project.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Home";
            return View();
        }
        //public IActionResult About()
        //{
        //    ViewData["Title"] = "About";   
        // return View();
        //}
    }
}
