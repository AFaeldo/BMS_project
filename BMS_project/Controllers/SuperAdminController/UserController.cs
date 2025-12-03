using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BMS_project.Data;
using BMS_project.Models;
using BMS_project.Models.Dto;
using System.Security.Claims;
using BMS_project.Services;

namespace BMS_project.Controllers.SuperAdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly ISystemLogService _systemLogService;

        public UsersController(ApplicationDbContext context, ISystemLogService systemLogService)
        {
            _context = context;
            _systemLogService = systemLogService;
            _passwordHasher = new PasswordHasher<User>();
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            // Fetch users and their active service record term
            // Note: We assume one active service record per user ideally.
            var query = _context.Login
                .Include(l => l.User)
                    .ThenInclude(u => u.ServiceRecords)
                        .ThenInclude(sr => sr.KabataanTermPeriod)
                .Include(l => l.User)
                    .ThenInclude(u => u.Barangay)
                .Include(l => l.Role)
                .Where(l => l.User != null && !l.User.IsArchived) // Filter active users
                .OrderBy(l => l.Username);

             // Pagination optional here or handled by caller. The previous code didn't use 'page' param in logic effectively 
             // (it returned all), but I'll stick to previous logic which returned all for JS DataTable usually.
             // Wait, the JS code calls `api.list` with `{ page }`.
             // But previous implementation of `GetAll` returned ALL. 
             // The JS `renderRows` handles the data. 
             // Actually, the `SuperAdminController` `Barangay` method had pagination server side.
             // This API controller `GetAll` previously returned `Ok(data)` (List). 
             // I will keep it returning List as per previous implementation for now.

            var data = await query.Select(l => new UserListDto
                {
                    Id = l.Id,
                    Username = l.Username,
                    LastName = l.User != null ? l.User.Last_Name : null,
                    FirstName = l.User != null ? l.User.First_Name : null,
                    BarangayId = l.User != null ? l.User.Barangay_ID : null,
                    BarangayName = l.User != null && l.User.Barangay != null ? l.User.Barangay.Barangay_Name : null,
                    RoleId = l.Role_ID,
                    Role = l.Role != null ? l.Role.Role_Name : null,
                    Email = l.User != null ? l.User.Email : null,
                    Term = l.User.ServiceRecords.Where(sr => sr.Status == "Active").Select(sr => sr.KabataanTermPeriod.Term_Name).FirstOrDefault() ?? "No Active Term",
                    IsActive = true
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("archived")]
        public async Task<IActionResult> GetArchived()
        {
            var data = await _context.Login
                .Include(l => l.User)
                .Include(l => l.Role)
                .Where(l => l.User != null && l.User.IsArchived) // Filter archived users
                .Select(l => new UserListDto
                {
                    Id = l.Id,
                    Username = l.Username,
                    LastName = l.User != null ? l.User.Last_Name : null,
                    FirstName = l.User != null ? l.User.First_Name : null,
                    BarangayId = l.User != null ? l.User.Barangay_ID : null,
                    BarangayName = l.User != null && l.User.Barangay != null ? l.User.Barangay.Barangay_Name : null,
                    RoleId = l.Role_ID,
                    Role = l.Role != null ? l.Role.Role_Name : null,
                    Email = l.User != null ? l.User.Email : null,
                    IsActive = false
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("generate-username")]
        public async Task<IActionResult> GenerateUsername()
        {
            var suffixes = await _context.Login
                .Where(l => l.Username != null && l.Username.StartsWith("SK"))
                .Select(l => l.Username.Substring(2))
                .ToListAsync();

            var maxSuffix = suffixes
                .Where(s => !string.IsNullOrEmpty(s) && s.All(char.IsDigit))
                .Select(s => int.Parse(s))
                .DefaultIfEmpty(0)
                .Max();

            var next = maxSuffix + 1;
            var newUsername = $"SK{next:D4}";

            return Ok(new { username = newUsername });
        }

        // GET: api/users/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var login = await _context.Login
                .Include(l => l.User)
                .Include(l => l.Role)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (login == null) return NotFound();

            var dto = new UserEditDto
            {
                Id = login.Id,
                Username = login.Username,
                FirstName = login.User?.First_Name,
                LastName = login.User?.Last_Name,
                Email = login.User?.Email,
                BarangayId = login.User?.Barangay_ID,
                RoleId = login.Role_ID
            };

            return Ok(dto);
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserEditDto dto)
        {
            if (dto == null) return BadRequest();

            // Basic checks
            if (string.IsNullOrWhiteSpace(dto.Username)) return BadRequest("Username required.");
            if (string.IsNullOrWhiteSpace(dto.Password)) return BadRequest("Password required.");
            if (dto.RoleId == null) return BadRequest("Role is required.");
            if (dto.BarangayId == null) return BadRequest("Barangay is required.");
            if (string.IsNullOrWhiteSpace(dto.Email)) return BadRequest("Email Is Required");

            // Check active term
            var activeTerm = await _context.KabataanTermPeriods.FirstOrDefaultAsync(t => t.IsActive);
            if (activeTerm == null)
            {
                return BadRequest("No Active Term Period found. Please set a term in the Dashboard first.");
            }

            // check duplicate username
            if (await _context.Login.AnyAsync(l => l.Username == dto.Username))
                return BadRequest("Username already exists.");

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // create user
                var user = new User
                {
                    First_Name = dto.FirstName,
                    Last_Name = dto.LastName,
                    Email = dto.Email,
                    Barangay_ID = dto.BarangayId,
                    Role_ID = dto.RoleId,
                    IsArchived = false
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // create login
                var login = new Login
                {
                    Username = dto.Username,
                    Role_ID = dto.RoleId ?? 0,
                    User_ID = user.User_ID
                };
                login.Password = _passwordHasher.HashPassword(user, dto.Password);

                _context.Login.Add(login);

                // create service record
                var serviceRecord = new KabataanServiceRecord
                {
                    User_ID = user.User_ID,
                    Term_ID = activeTerm.Term_ID,
                    Role_ID = dto.RoleId ?? 0,
                    Status = "Active"
                };
                _context.KabataanServiceRecords.Add(serviceRecord);

                // LOGGING
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdStr, out int adminId))
                {
                    await _systemLogService.LogAsync(adminId, "Create User", $"Created User: {dto.Username}", "User", user.User_ID);
                }

                await _context.SaveChangesAsync();

                await tx.CommitAsync();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/users/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserEditDto dto)
        {
            if (dto == null) return BadRequest();

            // find login and user
            var login = await _context.Login.FirstOrDefaultAsync(l => l.Id == id);
            if (login == null) return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.User_ID == login.User_ID);
            if (user == null) return NotFound();

            // duplicate username check (exclude this login)
            if (!string.IsNullOrWhiteSpace(dto.Username))
            {
                var other = await _context.Login.AsNoTracking().FirstOrDefaultAsync(l => l.Username == dto.Username);
                if (other != null && other.Id != id)
                    return BadRequest("Username already exists.");
            }

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // update user fields
                user.First_Name = dto.FirstName;
                user.Last_Name = dto.LastName;
                user.Email = dto.Email;
                user.Barangay_ID = dto.BarangayId;
                user.Role_ID = dto.RoleId;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                // update login fields
                login.Username = dto.Username;
                login.Role_ID = dto.RoleId ?? login.Role_ID;

                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    login.Password = _passwordHasher.HashPassword(user, dto.Password);
                }

                _context.Login.Update(login);

                // LOGGING
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdStr, out int adminId))
                {
                    await _systemLogService.LogAsync(adminId, "Update User", $"Updated User: {dto.Username}", "User", user.User_ID);
                }

                await _context.SaveChangesAsync();
                
                await tx.CommitAsync();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/users/5/archive (Resign/Step Down)
        [HttpPost("{id:int}/archive")]
        public async Task<IActionResult> Archive(int id)
        {
            var login = await _context.Login
                .Include(l => l.User)
                .Include(l => l.Role)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (login == null || login.User == null) return NotFound();

            // Role Restriction: This archiving action is only allowed for users with the "Barangay" role.
            if (login.Role == null || !login.Role.Role_Name.Contains("Barangay", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only users with the 'Barangay' role can be archived.");
            }

            // Constraint: A user cannot resign if they have any projects with status 'Pending' or 'Approved'.
            var hasOngoingProjects = await _context.Projects.AnyAsync(p => 
                p.User_ID == login.User_ID && 
                (p.Project_Status == "Pending" || p.Project_Status == "Approved"));

            if (hasOngoingProjects)
            {
                return BadRequest("Cannot resign while there are ongoing projects or proposals.");
            }

            await using var tx = await _context.Database.BeginTransactionAsync();
            try 
            {
                // 1. Set User to Archived
                login.User.IsArchived = true;

                // 2. Find Active Service Record and mark as Resigned
                var activeRecord = await _context.KabataanServiceRecords
                    .Where(r => r.User_ID == login.User_ID && r.Status == "Active")
                    .OrderByDescending(r => r.Record_ID)
                    .FirstOrDefaultAsync();

                if (activeRecord != null)
                {
                    activeRecord.Status = "Resigned";
                    activeRecord.Actual_End_Date = DateTime.Now;
                    _context.KabataanServiceRecords.Update(activeRecord);
                }

                // LOGGING
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdStr, out int adminId))
                {
                    await _systemLogService.LogAsync(adminId, "Archive User", $"Resigned/Archived User: {login.Username}", "User", login.User_ID);
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch(Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, ex.Message);
            }

            return Ok(new { success = true });
        }

        // POST: api/users/restore (Re-Elect Selected)
        [HttpPost("restore")]
        public async Task<IActionResult> Restore([FromBody] int[] ids)
        {
            if (ids == null || ids.Length == 0) return BadRequest("No users selected.");

            var activeTerm = await _context.KabataanTermPeriods.FirstOrDefaultAsync(t => t.IsActive);
            if (activeTerm == null) return BadRequest("No Active Term Period found. Cannot re-elect users.");

            var logins = await _context.Login
                .Include(l => l.User)
                .Where(l => ids.Contains(l.Id))
                .ToListAsync();

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int.TryParse(userIdStr, out int adminId);

                foreach (var l in logins)
                {
                    if (l.User != null)
                    {
                        // Un-archive user
                        l.User.IsArchived = false;

                        // Create NEW Service Record (Re-election)
                        var newRecord = new KabataanServiceRecord
                        {
                            User_ID = l.User.User_ID,
                            Term_ID = activeTerm.Term_ID,
                            Role_ID = l.Role_ID,
                            Status = "Active"
                        };
                        _context.KabataanServiceRecords.Add(newRecord);

                        // Log for each user
                        if (adminId > 0)
                        {
                            await _systemLogService.LogAsync(adminId, "Restore User", $"Re-elected User: {l.Username}", "User", l.User.User_ID);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/users/barangays
        [HttpGet("barangays")]
        public async Task<IActionResult> Barangays()
        {
            var list = await _context.barangays
                .OrderBy(b => b.Barangay_Name)
                .Select(b => new { id = b.Barangay_ID, text = b.Barangay_Name })
                .ToListAsync();
            return Ok(list);
        }

        // GET: api/users/roles
        [HttpGet("roles")]
        public async Task<IActionResult> Roles()
        {
            var list = await _context.Roles
                .OrderBy(r => r.Role_Name)
                .Select(r => new { id = r.Role_ID, text = r.Role_Name })
                .ToListAsync();
            return Ok(list);
        }
    }
}
