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
                    Role_ID = dto.RoleId
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

        // DELETE: api/users/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var login = await _context.Login.FirstOrDefaultAsync(l => l.Id == id);
            if (login == null) return NotFound();

            // If cascade is set (login -> user), you may want to delete user instead.
            // Here we'll remove login and (optionally) user if present:
            User user = null;
            if (login.User_ID == null || login.User_ID is DBNull)
                user = null;
            else
                user = await _context.Users.FirstOrDefaultAsync(u => u.User_ID == login.User_ID);

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Login.Remove(login);
                await _context.SaveChangesAsync();

                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                }

                await tx.CommitAsync();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/users/barangays
        [HttpGet("barangays")]
        public async Task<IActionResult> Barangays()
        {
            var list = await _context.Barangays
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
