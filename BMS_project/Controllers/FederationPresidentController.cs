using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using BMS_project.Models;
using System.Collections.Generic;

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
            var projects = new List<ProjectApprovalViewModel>();

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
                        projects.Add(new ProjectApprovalViewModel
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
    }
}
