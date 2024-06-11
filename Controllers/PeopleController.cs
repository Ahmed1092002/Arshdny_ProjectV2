using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Arshdny_ProjectV2.AppDbContext;
using Arshdny_ProjectV2.Models;
using Microsoft.VisualBasic;
using System.Net.Mail;

namespace Arshdny_Project.Controllers
{
    public class PersonDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public string Address { get; set; }

        public string Phone1 { get; set; }

        public string Phone2 { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Email { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly AppDbContextt _context;


        // Helper method to validate email address
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


       
        public PeopleController(AppDbContextt context)
        {
            _context = context;
        }

        // GET: api/People
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersons()
        {
          if (_context.Persons == null)
          {
              return NotFound();
          }
            return await _context.Persons.ToListAsync();
                

        }

        // GET: api/People/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
          if (_context.Persons == null)
          {
              return NotFound();
          }
            var person = await _context.Persons.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            return person;
        }

        // PUT: api/People/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutPerson(Person person)
        {
            if (!PersonExists(person.PersonId))
            {
                return NotFound($"Person With PersonID{person.PersonId}");
            }

            // Check if _context.Persons is null (although this check might not be necessary in normal circumstances)
            if (_context.Persons == null)
            {
                return Problem("Entity set 'DbContext.Persons' is null.");
            }

            
            // Validate Phone1 - allow only numbers from 0 to 9
            if (!string.IsNullOrEmpty(person.Phone1) && !person.Phone1.All(char.IsDigit))
            {
                return BadRequest("Phone number should only contain digits (0-9).");
            }

            // Clear Phone2 if it's an invalid string or null/empty/whitespace
            if (person.Phone2 == "string" || string.IsNullOrEmpty(person.Phone2) || string.IsNullOrWhiteSpace(person.Phone2))
            {
                person.Phone2 = null;
            }

            // Validate DateOfBirth - do not accept future dates
            if (person.DateOfBirth > DateTime.Today)
            {
                return BadRequest("Date of birth cannot be a future date.");
            }

            // Validate FirstName, LastName, and Address
            if (string.IsNullOrEmpty(person.FirstName) || person.FirstName.ToLower() == "string" ||
                string.IsNullOrEmpty(person.LastName) || person.LastName.ToLower() == "string" ||
                string.IsNullOrEmpty(person.Address) || person.Address.ToLower() == "string" ||
                string.IsNullOrEmpty(person.Phone1) || person.Phone1.ToLower() == "string" ||
                string.IsNullOrEmpty(person.Email) || person.Email.ToLower() == "string")
            {
                return BadRequest("There is empty or 'string' fileds");
            }

            // Validate Email
            if (!IsValidEmail(person.Email))
            {
                return BadRequest("Invalid email address.");
            }



            _context.Entry(person).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetPerson), new { id = person.PersonId }, person);

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(person.PersonId))
                {
                    return NotFound($"Person With PersonID{person.PersonId}");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Persons
        [HttpPost]
        public async Task<ActionResult<Person>> PostPerson(PersonDto personDto)
        {
            try
            {
                // Check if _context.Persons is null (although this check might not be necessary in normal circumstances)
                if (_context.Persons == null)
                {
                    return Problem("Entity set 'AppDbContext.Persons' is null.");
                }

                // Check if the phone1 already exists
                if (await _context.Persons.AnyAsync(p => p.Phone1 == personDto.Phone1))
                {
                    return Conflict("Phone number already exists.");
                }

                // Validate Phone1 - allow only numbers from 0 to 9
                if (!string.IsNullOrEmpty(personDto.Phone1) && !personDto.Phone1.All(char.IsDigit))
                {
                    return BadRequest("Phone number should only contain digits (0-9).");
                }

                // Clear Phone2 if it's an invalid string or null/empty/whitespace
                if (personDto.Phone2 == "string" || string.IsNullOrEmpty(personDto.Phone2) || string.IsNullOrWhiteSpace(personDto.Phone2))
                {
                    personDto.Phone2 = null;
                }

                // Validate DateOfBirth - do not accept future dates
                if (personDto.DateOfBirth > DateTime.Today)
                {
                    return BadRequest("Date of birth cannot be a future date.");
                }

                // Validate FirstName, LastName, and Address
                if (string.IsNullOrEmpty(personDto.FirstName) || personDto.FirstName.ToLower() == "string" ||
                    string.IsNullOrEmpty(personDto.LastName) || personDto.LastName.ToLower() == "string" ||
                    string.IsNullOrEmpty(personDto.Address) || personDto.Address.ToLower() == "string" ||
                    string.IsNullOrEmpty(personDto.Phone1) || personDto.Phone1.ToLower() == "string" ||
                    string.IsNullOrEmpty(personDto.Email) || personDto.Email.ToLower() == "string")
                {
                    return BadRequest("There are empty or 'string' fields.");
                }

                // Validate Email
                if (!IsValidEmail(personDto.Email))
                {
                    return BadRequest("Invalid email address.");
                }

                // Map PersonDto to Person entity
                var person = new Person
                {
                    
                    FirstName = personDto.FirstName,
                    LastName = personDto.LastName,
                    Gender = personDto.Gender,
                    Address = personDto.Address,
                    Phone1 = personDto.Phone1,
                    Phone2 = personDto.Phone2,
                    DateOfBirth = personDto.DateOfBirth,
                    Email = personDto.Email
                };

                // Add the person to the context and save changes
                _context.Persons.Add(person);
                await _context.SaveChangesAsync();

                // Return a created response
                return CreatedAtAction(nameof(GetPerson), new { id = person.PersonId }, person);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request: " + ex.Message);
            }
        }

        // DELETE: api/People/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            if (_context.Persons == null)
            {
                return NotFound();
            }
            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PersonExists(int id)
        {
            return (_context.Persons?.Any(e => e.PersonId == id)).GetValueOrDefault();
        }
    }
}
