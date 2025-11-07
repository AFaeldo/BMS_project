using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using BMS_project.Models;
using System.Collections.Generic;
using System;

namespace BMS_project.Controllers
{
    public class FederationPresidentController : Controller
    {
        private readonly string connectionString = "server=localhost;database=kabataan;uid=root;pwd=;";

        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Administrative Dashboard";
            return View();
        }

        public IActionResult ComplianceMonitoring()
        {
            ViewData["Title"] = "Compliance Monitoring";
            return View();
        }

        public IActionResult ReportGeneration()
        {
            ViewData["Title"] = "Report Generation";
            return View();
        }

        public IActionResult Notifications()
        {
            ViewData["Title"] = "Notifications";
            return View();
        }

        //  Updated: Project Approvals now pulls data from MySQL
        public IActionResult ProjectApprovals()
        {
            ViewData["Title"] = "Project Approvals";
            var projects = new List<ProjectApprovalModel>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT 
                        p.Project_ID,
                        p.Project_Title,
                        p.Project_Description,
                        p.Date_Submitted,
                        p.Project_Status,
                        b.Barangay_Name
                    FROM project p
                    LEFT JOIN user u ON p.User_ID = u.User_ID
                    LEFT JOIN barangay b ON u.Barangay_ID = b.Barangay_ID
                    ORDER BY p.Date_Submitted DESC;
                ";

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        projects.Add(new ProjectApprovalModel
                        {
                            Project_ID = reader.GetInt32("Project_ID"),
                            Project_Title = reader["Project_Title"].ToString(),
                            Project_Description = reader["Project_Description"]?.ToString(),
                            Date_Submitted = reader["Date_Submitted"] as DateTime?,
                            Project_Status = reader["Project_Status"].ToString(),
                            Barangay_Name = reader["Barangay_Name"]?.ToString()
                        });
                    }
                }
            }

            return View(projects);
        }

        // Handles Approve / Reject actions
        [HttpPost]
        public IActionResult UpdateStatus(int projectId, string status, string remarks)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Update project table
                string updateQuery = "UPDATE project SET Project_Status = @status WHERE Project_ID = @id";
                using (var cmd = new MySqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@id", projectId);
                    cmd.ExecuteNonQuery();
                }

                // Insert into project_log
                string logQuery = @"INSERT INTO project_log (Project_ID, Status, Remarks)
                                    VALUES (@id, @status, @remarks)";
                using (var cmd2 = new MySqlCommand(logQuery, conn))
                {
                    cmd2.Parameters.AddWithValue("@id", projectId);
                    cmd2.Parameters.AddWithValue("@status", status);
                    cmd2.Parameters.AddWithValue("@remarks", remarks ?? "");
                    cmd2.ExecuteNonQuery();
                }
            }

            return RedirectToAction("ProjectApprovals");
        }

        public IActionResult Budget()
        {
            ViewData["Title"] = "Barangay Budgets";

            var barangayBudgets = new List<dynamic>();

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                SELECT 
                    b.Barangay_Name,
                    IFNULL(bb.Allotment, 0) AS Allotment,
                    IFNULL(bb.Disbursed, 0) AS Disbursed
                FROM barangay b
                LEFT JOIN barangay_budget bb ON b.Barangay_ID = bb.Barangay_ID
                ORDER BY b.Barangay_Name ASC;
            ";

                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            barangayBudgets.Add(new
                            {
                                BarangayName = reader["Barangay_Name"].ToString(),
                                Allotment = Convert.ToDecimal(reader["Allotment"]),
                                Disbursed = Convert.ToDecimal(reader["Disbursed"]),
                                Details = new List<dynamic>()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading barangay budgets: " + ex.Message;
            }

            ViewBag.BarangayBudgets = barangayBudgets;

            return View("Budget"); // ✅ Correct View
        }

        public IActionResult Profile()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SaveProfile(string Barangay, string PostalAddress, string Zone, string District, string City)
        {
            TempData["SuccessMessage"] = "Profile saved successfully!";
            return RedirectToAction("Profile");
        }

        public IActionResult UserManagement()
        {
            ViewData["Title"] = "User Management";
            return View();
        }
    }
}
