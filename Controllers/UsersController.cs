using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Arshdny_ProjectV2.AppDbContext;
using Arshdny_ProjectV2.Models;
using Arshdny_ProjectV2.Utilites;

namespace Arshdny_Project.Controllers
{
    public class UserDto
    {
        public int PersonId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool IsBlocked { get; set; }
    }
    

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContextt _context;

        public UsersController(AppDbContextt context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutUser(User user)
        {
            if (!UserExists(user.UserId))
            {
                return NotFound();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }




        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserDto userDto)
        {
            try
            {

                // Map UserDto to User entity
                var user = new User
                {
                    PersonId = userDto.PersonId,
                    UserName = userDto.UserName,
                    Password = userDto.Password,
                    IsBlocked = userDto.IsBlocked,
                    CreatedAt = DateTime.UtcNow // Set current time for CreatedAt
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Return a simplified response without sensitive data
                return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, new
                {
                    user.UserId,
                    user.PersonId,
                    user.UserName,
                    user.IsBlocked
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request: " + ex.Message);
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }

       

        private bool VerifyPassword(User user, string password)
        {
            
            return user.Password == password;
        }

        // PUT: api/Users/BlockUser/5
        [HttpPut("BlockUser/{id}")]
        public async Task<IActionResult> BlockUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsBlocked = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/Users/UnblockUser/5
        [HttpPut("UnblockUser/{id}")]
        public async Task<IActionResult> UnblockUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsBlocked = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Users/IsBlocked/5
        [HttpGet("IsBlocked/{id}")]
        public async Task<ActionResult<bool>> IsBlocked(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return user.IsBlocked;
        }

        // GET: api/Users/NumberOfUsers
        [HttpGet("NumberOfUsers")]
        public async Task<ActionResult<int>> CountNumberOfUsers()
        {
            var count = await _context.Users.CountAsync();
            return count;
        }

        // GET: api/Users/NumberOfUsersPerMonth
        [HttpGet("NumberOfUsersPerMonth")]
        public async Task<ActionResult<IEnumerable<object>>> CountNumberOfUsersPerMonth()
        {
            var userCounts = await _context.Users
                .GroupBy(u => new { Year = u.CreatedAt.Year, Month = u.CreatedAt.Month })
                .Select(g => new { YearMonth = new DateTime(g.Key.Year, g.Key.Month, 1), Count = g.Count() })
                .ToListAsync();

            return userCounts;
        }

      

        // GET: api/Users/GetUsersPerYear
        [HttpGet("GetUsersPerYear")]
        public async Task<ActionResult<IEnumerable<object>>> GetUsersPerYear(int Year)
        {
            var userCounts = await _context.Users
                .Where(u => u.CreatedAt.Year == Year)
                .GroupBy(u => new { Year = u.CreatedAt.Year, Month = u.CreatedAt.Month })
                .Select(g => new
                {
                    YearMonth = $"{g.Key.Year}-{g.Key.Month:D2}", // Format YearMonth as "YYYY-MM"
                    Count = g.Count()
                })
                .ToListAsync();

            return userCounts;
        }


        // GET: api/Users/GetUsersPerDate
        [HttpGet("GetUsersPerSpecificDate")]
        public async Task<ActionResult<IEnumerable<object>>> GetUsersPerSpecificDate(int Year, int Month, int Day)
        {
            var userDetails = await _context.Users
                .Where(u => u.CreatedAt.Year == Year && u.CreatedAt.Month == Month && u.CreatedAt.Day == Day)
                .Select(u => new
                {
                    u.UserId,
                    u.PersonId,
                    u.UserName,
                    u.IsBlocked,
                    u.CreatedAt
                })
                .ToListAsync();

            var userCounts = userDetails
                .GroupBy(u => new { Year = u.CreatedAt.Year, Month = u.CreatedAt.Month, Day = u.CreatedAt.Day })
                .Select(g => new
                {
                    Date = $"{g.Key.Year}-{g.Key.Month:D2}-{g.Key.Day:D2}", // Format YearMonthDay as "YYYY-MM-DD"
                    Count = g.Count(),
                    Users = g.ToList()
                })
                .ToList();

            return userCounts;
        }

        // GET: api/Users/GetUsersPerMonth
        [HttpGet("GetUsersPerMonth")]
        public async Task<ActionResult<IEnumerable<object>>> GetUsersPerMonth(int Year, int Month)
        {
            var userDetails = await _context.Users
                .Where(u => u.CreatedAt.Year == Year && u.CreatedAt.Month == Month)
                .Select(u => new
                {
                    u.UserId,
                    u.PersonId,
                    u.UserName,
                    u.IsBlocked,
                    u.CreatedAt
                })
                .ToListAsync();

            var userCounts = userDetails
                .GroupBy(u => new { Year = u.CreatedAt.Year, Month = u.CreatedAt.Month })
                .Select(g => new
                {
                    YearMonth = $"{g.Key.Year}-{g.Key.Month:D2}", // Format YearMonth as "YYYY-MM"
                    Count = g.Count(),
                    Users = g.ToList()
                })
                .ToList();

            return userCounts;
        }






    }
}
