﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourProjectName.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        public IActionResult BackupMaintenance()
        {
            ViewData["Title"] = "Backup & Maintenance";
            return View();
        }
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Dashboard";
            return View();
        }

        public IActionResult ManageUsers()
        {
            ViewData["Title"] = "Manage Users";
            return View();
        }

        public IActionResult SystemLogs()
        {
            ViewData["Title"] = "System Logs";
            return View();
        }

        public IActionResult Settings()
        {
            ViewData["Title"] = "Settings";
            return View();
        }
    }
}
