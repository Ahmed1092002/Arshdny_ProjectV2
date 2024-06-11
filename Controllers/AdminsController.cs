using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Arshdny_ProjectV2.AppDbContext;
using Arshdny_ProjectV2.Models;
using Arshdny_ProjectV2.DtoModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Arshdny_ProjectV2.Utilites;

namespace Arshdny_Project.Controllers
{

    public class AdminLoginResponse
    {
        public Admin Admin { get; set; }
        public User User { get; set; }
        public Person Person { get; set; }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly AppDbContextt _context;

        public AdminsController(AppDbContextt context)
        {
            _context = context;
        }

        // GET: api/Admins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAdmins()
        {
          if (_context.Admins == null)
          {
              return NotFound();
          }
            return await _context.Admins.ToListAsync();
        }

        // GET: api/Admins/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Admin>> GetAdmin(int id)
        {
          if (_context.Admins == null)
          {
              return NotFound();
          }
            var admin = await _context.Admins.FindAsync(id);

            if (admin == null)
            {
                return NotFound();
            }

            return admin;
        }

        // PUT: api/Admins/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutAdmin( Admin admin)
        {
            if (!AdminExists(admin.AdminId))
            {
                return NotFound();
            }

            _context.Entry(admin).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminExists(admin.AdminId))
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

        // POST: api/Admins
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Admin>> PostAdmin(AdminDto adminDto,int CurrentAdminID)
        {
            if (_context.Admins == null)
            {
                return Problem("Entity set 'AppDbContext.Admins' is null.");
            }

            var currentAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.AdminId == CurrentAdminID);
            if(currentAdmin!=null)
            {
                if (currentAdmin.Permission == 0)
                    return BadRequest("Access Denied You Dont have Permission to add Admin");
            }
            else
            {
                return NotFound();
            }

            // Validate input fields
            if (adminDto.Qualification == "string" || string.IsNullOrEmpty(adminDto.Qualification) || string.IsNullOrWhiteSpace(adminDto.Qualification))
            {
                return BadRequest("Qualification cannot be 'string' or null.");
            }

            if (adminDto.Roles == "string" || string.IsNullOrEmpty(adminDto.Roles) || string.IsNullOrWhiteSpace(adminDto.Roles))
            {
                return BadRequest("Roles cannot be 'string' or null.");
            }



            // Map AdminDto to Admin
            var admin = new Admin
            {
                UserId = adminDto.UserId,
                Qualification = adminDto.Qualification,
                Roles = adminDto.Roles,
                Permission = adminDto.Permission
            };

            var refugees =  _context.Refugees.Where(r => r.UserId == adminDto.UserId).ToList();

            if (!refugees.Any())// Check if the list is empty
            {
                _context.Admins.Add(admin);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetAdmin", new { id = admin.AdminId }, admin);
            }

            return BadRequest("UserID Already Exist in Refugees");
        }


        // DELETE: api/Admins/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            if (_context.Admins == null)
            {
                return NotFound();
            }
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdminExists(int id)
        {
            return (_context.Admins?.Any(e => e.AdminId == id)).GetValueOrDefault();
        }

        // GET: api/Users/NumberOfAdmins
        [HttpGet("NumberOfAdmins")]
        public async Task<ActionResult<int>> CountNumberOfAdmins()
        {
            var count = await _context.Admins.CountAsync();
            return count;
        }

        // GET: api/Admin/AdminLogin
        [HttpGet("AdminLogin")]
        public async Task<ActionResult> AdminLogin(string userName, string password)
        {
            try
            {

                // Execute the SQL query to retrieve the specific fields based on username and password
                var adminData = await (from user in _context.Users
                                       join admin in _context.Admins on user.UserId equals admin.UserId
                                       where user.UserName == userName && user.Password == password
                                       select new
                                       {
                                           admin.AdminId,
                                           admin.UserId,
                                           admin.Roles,
                                           admin.Permission,
                                           user.UserName
                                       })
                                       .FirstOrDefaultAsync();

                if (adminData == null)
                {
                    return NotFound("Admin not found with the provided username and password.");
                }
                // Return the authenticated admin data along with user data
                return Ok(adminData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request: " + ex.Message);
            }
        }




      

    }




}
