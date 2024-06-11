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

namespace Arshdny_Project.Controllers
{
    public class RefugeeDto
    {
        public int RefugeeJobId { get; set; }
        public int UserId { get; set; }
        public string RefugeeCardNo { get; set; } = null!;
        public int CountryId { get; set; }
        public int NationaltyId { get; set; }
        public string? Cv { get; set; }
        public string ImagePath { get; set; } = null!;
        public DateTime CardStartDate { get; set; }
        public DateTime CardEndDate { get; set; }
        public string DeviceToken { get; set; }
    }


    // Define a new class to hold both Refugee and Person data
    public class RefugeeLoginResponse
    {
        public Refugee Refugee { get; set; }
        public Person Person { get; set; }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class RefugeesController : ControllerBase
    {
        private readonly AppDbContextt _context;

        public RefugeesController(AppDbContextt context)
        {
            _context = context;
        }

        // GET: api/Refugees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Refugee>>> GetRefugees()
        {
          if (_context.Refugees == null)
          {
              return NotFound();
          }
            return await _context.Refugees.ToListAsync();
        }

        // GET: api/Refugees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Refugee>> GetRefugee(int id)
        {
          if (_context.Refugees == null)
          {
              return NotFound();
          }
            var refugee = await _context.Refugees.FindAsync(id);

            if (refugee == null)
            {
                return NotFound();
            }

            return refugee;
        }

        // PUT: api/Refugees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutRefugee(Refugee refugee)
        {
            if (!RefugeeExists(refugee.RefugeeId))
            {
                return NotFound();
            }

            if (refugee.Cv == "string" || string.IsNullOrEmpty(refugee.Cv) || string.IsNullOrWhiteSpace(refugee.Cv))
            {
                refugee.Cv = null;
            }
            // Validate input fields
            if (refugee.RefugeeCardNo == "string" || string.IsNullOrEmpty(refugee.RefugeeCardNo) || string.IsNullOrWhiteSpace(refugee.RefugeeCardNo))
            {
                return BadRequest("RefugeeCardNo cannot be 'string' or null.");
            }

            if (refugee.ImagePath == "string" || string.IsNullOrEmpty(refugee.ImagePath) || string.IsNullOrWhiteSpace(refugee.ImagePath))
            {
                return BadRequest("ImagePath cannot be 'string' or null.");
            }

            if (refugee.DeviceToken == "string" || string.IsNullOrEmpty(refugee.DeviceToken) || string.IsNullOrWhiteSpace(refugee.DeviceToken))
            {
                return BadRequest("DeviceToken cannot be 'string' or null.");
            }

            _context.Entry(refugee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RefugeeExists(refugee.RefugeeId))
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


        // POST: api/Refugees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Refugee>> PostRefugee(RefugeeDto refugeeDto)
        {
            if (_context.Refugees == null)
            {
                return Problem("Entity set 'AppDbContext.Refugees' is null.");
            }
            // Validate input fields
            if (refugeeDto.RefugeeCardNo == "string" || string.IsNullOrEmpty(refugeeDto.RefugeeCardNo) || string.IsNullOrWhiteSpace(refugeeDto.RefugeeCardNo))
            {
                return BadRequest("RefugeeCardNo cannot be 'string' or null.");
            }

            if (refugeeDto.ImagePath == "string" || string.IsNullOrEmpty(refugeeDto.ImagePath) || string.IsNullOrWhiteSpace(refugeeDto.ImagePath))
            {
                return BadRequest("ImagePath cannot be 'string' or null.");
            }

            if (refugeeDto.DeviceToken == "string" || string.IsNullOrEmpty(refugeeDto.DeviceToken) || string.IsNullOrWhiteSpace(refugeeDto.DeviceToken))
            {
                return BadRequest("DeviceToken cannot be 'string' or null.");
            }
            // Map RefugeeDto to Refugee entity
            var refugee = new Refugee
            {
                RefugeeJobId = refugeeDto.RefugeeJobId,
                UserId = refugeeDto.UserId,
                RefugeeCardNo = refugeeDto.RefugeeCardNo,
                CountryId = refugeeDto.CountryId,
                NationaltyId = refugeeDto.NationaltyId,
                Cv = refugeeDto.Cv == "string" || string.IsNullOrEmpty(refugeeDto.Cv) || string.IsNullOrWhiteSpace(refugeeDto.Cv) ? null : refugeeDto.Cv,
                ImagePath = refugeeDto.ImagePath,
                CardStartDate = refugeeDto.CardStartDate,
                CardEndDate = refugeeDto.CardEndDate,
                DeviceToken = refugeeDto.DeviceToken
            };


            var Admin = _context.Admins.Where(r => r.UserId == refugeeDto.UserId).ToList();

            if (!Admin.Any())// Check if the list is empty
            {
                _context.Refugees.Add(refugee);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetRefugee", new { id = refugee.RefugeeId }, refugee);
            }

            return BadRequest("UserID Already Exist in Admin");




            

        }

        // DELETE: api/Refugees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRefugee(int id)
        {
            if (_context.Refugees == null)
            {
                return NotFound();
            }
            var refugee = await _context.Refugees.FindAsync(id);
            if (refugee == null)
            {
                return NotFound();
            }

            _context.Refugees.Remove(refugee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // GET: api/Users/RefugeesByCountry/1
        [HttpGet("RefugeesByCountry/{countryId}")]
        public async Task<ActionResult<IEnumerable<Refugee>>> GetRefugeesByCountry(int countryId)
        {
            var refugees = await _context.Refugees.Where(r => r.CountryId == countryId).ToListAsync();
            if (refugees == null || !refugees.Any())
            {
                return NotFound("No refugees found for the specified country.");
            }

            return refugees;
        }

        // GET: api/Users/GetCountryRefugee/6
        [HttpGet("GetCountryRefugee/{refugeeId}")]
        public async Task<ActionResult<string>> GetCountryRefugee(int refugeeId)
        {
            var countryName = await (from refugee in _context.Refugees
                                     join country in _context.Countries on refugee.CountryId equals country.CountryId
                                     where refugee.RefugeeId == refugeeId
                                     select country.CountryName)
                                     .FirstOrDefaultAsync();

            if (countryName == null)
            {
                return NotFound("No country found for the specified refugee ID.");
            }

            return countryName;
        }
        private bool RefugeeExists(int id)
        {
            return (_context.Refugees?.Any(e => e.RefugeeId == id)).GetValueOrDefault();
        }


        // GET: api/Users/NumberOfRefugees
        [HttpGet("NumberOfRefugees")]
        public async Task<ActionResult<int>> CountNumberOfRefugees()
        {
            var count = await _context.Refugees.CountAsync();
            return count;
        }

        // GET: api/Refugees/CountByCountry/{countryName}
        [HttpGet("CountRegugeeByCountry/{countryName}")]
        public async Task<ActionResult<int>> CountRefugeesByCountry(string countryName)
        {
            // Find the country by name
            var country = await _context.Countries.FirstOrDefaultAsync(c => c.CountryName == countryName);

            if (country == null)
            {
                return NotFound("Country not found.");
            }

            // Count the number of refugees in the specified country
            var count = await _context.Refugees.CountAsync(r => r.CountryId == country.CountryId);

            return count;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RefugeeRegistrationModel model)
        {

            try
            {
                // Add person to the database
                var person = new Person
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Gender = model.Gender,
                    Address = model.Address,
                    Phone1 = model.Phone1,
                    Phone2 = model.Phone2,
                    DateOfBirth = model.DateOfBirth,
                    Email = model.Email
                };

                _context.Persons.Add(person);
                await _context.SaveChangesAsync();

                // Add user to the database
                var user = new User
                {
                    PersonId = person.PersonId,
                    UserName = model.UserName,
                    Password = model.Password,
                    IsBlocked = model.IsBlocked,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Add refugee to the database
                var refugee = new Refugee
                {
                    RefugeeJobId = model.RefugeeJobId,
                    UserId = user.UserId,
                    RefugeeCardNo = model.RefugeeCardNo,
                    CountryId = model.CountryId,
                    NationaltyId = model.NationaltyId,
                    Cv = model.Cv,
                    ImagePath = model.ImagePath,
                    CardStartDate = model.CardStartDate,
                    CardEndDate = model.CardEndDate
                };

                _context.Refugees.Add(refugee);
                await _context.SaveChangesAsync();

                return Ok("Registration successful!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while registering: " + ex.Message);
            }
        }


        // GET: api/Refugees/RefugeeLogin
        [HttpGet("RefugeeLogin")]
        public async Task<ActionResult> RefugeeLogin(string email, string password)
        {
            try
            {
                // Execute the SQL query to retrieve the specific fields based on email and password
                var refugeeData = await (from refugee in _context.Refugees
                                         join user in _context.Users on refugee.UserId equals user.UserId
                                         join person in _context.Persons on user.PersonId equals person.PersonId
                                         join country in _context.Countries on refugee.CountryId equals country.CountryId
                                         where person.Email == email && user.Password == password
                                         select new
                                         {
                                             refugee.RefugeeId,
                                             person.PersonId,
                                             user.UserId,
                                             country.CountryId,
                                             country.CountryName,
                                             refugee.Cv,
                                             refugee.ImagePath
                                         })
                                         .FirstOrDefaultAsync();

                if (refugeeData == null)
                {
                    return NotFound("Refugee not found with the provided email and password.");
                }

                // Return the authenticated refugee data
                return Ok(refugeeData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request: " + ex.Message);
            }
        }

        // GET: api/Refugees/IsRefugeeCardNoExist
        [HttpGet("IsRefugeeCardNoExist")]
        public async Task<ActionResult<bool>> IsRefugeeCardNoExist([FromQuery] string RefugeeCardNo)
        {
            try
            {
                // Check if the RefugeeCardNo exists in the database
                var refugee = await _context.Refugees.FirstOrDefaultAsync(r => r.RefugeeCardNo == RefugeeCardNo);

                // Return true if refugee is found, otherwise return false
                return Ok(refugee != null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request: " + ex.Message);
            }
        }

        [HttpGet("RefugeesByCountry")]
        public async Task<ActionResult<IEnumerable<object>>> GetRefugeesByCountry()
        {
            if (_context.Refugees == null)
            {
                return NotFound("No refugees available.");
            }

            var refugeesByCountry = await _context.Refugees
                .GroupBy(r => r.CountryId)
                .Select(group => new
                {
                    CountryName = _context.Countries.FirstOrDefault(c => c.CountryId == group.Key).CountryName,
                    Count = group.Count()
                })
                .ToListAsync();

            return refugeesByCountry;
        }

       

        // GET: api/Refugees/GetAllRefugeesAppliedOnThisJob
        [HttpGet("GetAllRefugeesAppliedOnThisJob")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllRefugeesAppliedOnThisJob(int JobID)
        {
            var refugees = await (from ra in _context.RefugeeAppliedJobs
                                  join r in _context.Refugees on ra.RefugeeId equals r.RefugeeId
                                  where ra.JobId == JobID
                                  select r).ToListAsync();


            return refugees;
        }

        // GET: api/Users/NumberOfRefugeesPerMonth
        [HttpGet("NumberOfRefugeesPerMonth")]
        public async Task<ActionResult<IEnumerable<object>>> NumberOfRefugeesPerMonth()
        {
            var refugeeCounts = await _context.Refugees
                .Join(
                    _context.Users,
                    refugee => refugee.UserId,
                    user => user.UserId,
                    (refugee, user) => new { Refugee = refugee, User = user }
                )
                .GroupBy(x => new { Year = x.User.CreatedAt.Year, Month = x.User.CreatedAt.Month })
                .Select(g => new
                {
                    YearMonth = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Count = g.Count()
                })
                .ToListAsync();

            return Ok(refugeeCounts);
        }




    }

}

