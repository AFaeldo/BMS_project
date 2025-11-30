using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BMS_project.Data;
using BMS_project.Models;
using BMS_project.Models.Dto;

namespace BMS_project.Controllers.SuperAdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Login
                .Include(l => l.User)
                .Include(l => l.Role)
                .Where(l => l.User != null && !l.User.IsArchived) // Filter active users
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

        // DELETE: api/users/5/archive (Soft Delete)
        [HttpPost("{id:int}/archive")]
        public async Task<IActionResult> Archive(int id)
        {
            var login = await _context.Login.Include(l => l.User).FirstOrDefaultAsync(l => l.Id == id);
            if (login == null || login.User == null) return NotFound();

            login.User.IsArchived = true;
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        // POST: api/users/restore
        [HttpPost("restore")]
        public async Task<IActionResult> Restore([FromBody] int[] ids)
        {
            if (ids == null || ids.Length == 0) return BadRequest("No users selected.");

            // Note: We receive Login IDs, but IsArchived is on User.
            // We need to find users associated with these login IDs.
            var logins = await _context.Login
                .Include(l => l.User)
                .Where(l => ids.Contains(l.Id))
                .ToListAsync();

            foreach (var l in logins)
            {
                if (l.User != null)
                {
                    l.User.IsArchived = false;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }

        // DELETE: api/users/5 (Kept for backward compatibility or hard delete if needed, but for now, let's make it return 405 or alias to archive if desired. 
        // Requirement says "Change logic... do NOT use Remove". So I will replace this method body to use soft delete logic too, but typically HTTP verb implies meaning.
        // I'll keep this as Archive logic but via Delete verb if existing frontend uses DELETE.)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
             return await Archive(id);
        }


        // GET: api/users/barangays
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
