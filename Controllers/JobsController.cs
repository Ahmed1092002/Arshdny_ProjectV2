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
using System.Security.Cryptography;

namespace Arshdny_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly AppDbContextt _context;

        public JobsController(AppDbContextt context)
        {
            _context = context;
        }

        // GET: api/Jobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
        {
          if (_context.Jobs == null)
          {
              return NotFound();
          }
            return await _context.Jobs.ToListAsync();
        }

        // GET: api/Jobs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetJob(int id)
        {
          if (_context.Jobs == null)
          {
              return NotFound();
          }
            var job = await _context.Jobs.FindAsync(id);

            if (job == null)
            {
                return NotFound();
            }

            return job;
        }

        // PUT: api/Jobs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // PUT: api/Jobs/EditIsViewJobStatus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("EditIsViewJobStatus/{jobId}")]
        public async Task<IActionResult> EditIsViewJobStatus(int jobId, bool isView)
        {
            // Check if the job exists
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null)
            {
                return NotFound();
            }

            // Update the IsView property
            job.IsView = isView;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobExists(jobId))
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

        [HttpPost]
        public async Task<ActionResult<Job>> PostJob(JobDto jobDto)
        {

           
            if (_context.Jobs == null)
            {
                return Problem("Entity set 'AppDbContext.Jobs' is null.");
            }
            // Validation for "string" values
            if (jobDto.Location == "string" || jobDto.Country == "string" || jobDto.Description == "string")
            {
                return BadRequest("Location, Address, and Description fields cannot be 'string'.");
            }

            var job = new Job
            {
                JobName = jobDto.JobName,
                Description = jobDto.Description,
                Salary = jobDto.Salary,
                Country = jobDto.Country,
                Location = jobDto.Location,
                PublishDate = jobDto.PublishDate,
                YearsOfExperience = jobDto.YearsOfExperience,
                IsView = jobDto.IsView
            };

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJob", new { id = job.JobId }, job);
        }


        // DELETE: api/Jobs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            if (_context.Jobs == null)
            {
                return NotFound();
            }
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JobExists(int id)
        {
            return (_context.Jobs?.Any(e => e.JobId == id)).GetValueOrDefault();
        }

        // GET: api/Users/NumberOfJobs
        [HttpGet("NumberOfJobs")]
        public async Task<ActionResult<int>> CountNumberOfJobs()
        {
            var count = await _context.Jobs.CountAsync();
            return count;
        }

       

        // GET: api/Jobs/CountJobsByjobName/{JobName}
        [HttpGet("CountJobsByjobName/{JobName}")]
        public async Task<ActionResult<int>> CountJobsByjobName(string JobName)
        {
            // Find the Job by Jobname
            var Job = await _context.Jobs.FirstOrDefaultAsync(c => c.JobName == JobName);

            if (Job == null)
            {
                return NotFound("Job not found.");
            }

            // Count the number of Jobs in the specified country
            var count = await _context.Jobs.CountAsync(r => r.JobId == Job.JobId);

            return count;
        }

        [HttpGet("CountPerJob")]
        public async Task<ActionResult<IEnumerable<Object>>> GetJobCounts()
        {
            if (_context.Jobs == null)
            {
                return NotFound("No jobs available.");
            }

            var jobCounts = await _context.Jobs
                .GroupBy(j => j.JobName) 
                .Select(group => new
                {
                    JobName = group.Key,
                    Count = group.Count()
                })
                .ToListAsync();

            return jobCounts;
        }


        // GET: api/Jobs/FilterJobs
        [HttpGet("FilterJobs")]
        public async Task<ActionResult> FilterJobs(
            [FromQuery] string? jobName,
            [FromQuery] string? description,
            [FromQuery] decimal? minSalary,
            [FromQuery] decimal? maxSalary,
            [FromQuery] decimal? specificSalary,
            [FromQuery] string? address,
            [FromQuery] string? location,
            [FromQuery] DateTime? minPublishDate,
            [FromQuery] DateTime? maxPublishDate,
            [FromQuery] DateTime? specificPublishDate,
            [FromQuery] int? yearsOfExperience)
        {
            try
            {
                var query = _context.Jobs.AsQueryable();

                if (!string.IsNullOrEmpty(jobName))
                {
                    query = query.Where(j => j.JobName.Contains(jobName));
                }

                if (!string.IsNullOrEmpty(description))
                {
                    query = query.Where(j => j.Description.Contains(description));
                }

                if (minSalary.HasValue)
                {
                    query = query.Where(j => j.Salary >= minSalary.Value);
                }

                if (maxSalary.HasValue)
                {
                    query = query.Where(j => j.Salary <= maxSalary.Value);
                }

                if (specificSalary.HasValue)
                {
                    query = query.Where(j => j.Salary == specificSalary.Value);
                }

                if (!string.IsNullOrEmpty(address))
                {
                    query = query.Where(j => j.Country.Contains(address));
                }

                if (!string.IsNullOrEmpty(location))
                {
                    query = query.Where(j => j.Location.Contains(location));
                }

                if (minPublishDate.HasValue)
                {
                    query = query.Where(j => j.PublishDate >= minPublishDate.Value);
                }

                if (maxPublishDate.HasValue)
                {
                    query = query.Where(j => j.PublishDate <= maxPublishDate.Value);
                }

                if (specificPublishDate.HasValue)
                {
                    query = query.Where(j => j.PublishDate.Date == specificPublishDate.Value.Date);
                }

                if (yearsOfExperience.HasValue)
                {
                    query = query.Where(j => j.YearsOfExperience == yearsOfExperience.Value);
                }

                var filteredJobs = await query
                    .Select(j => new
                    {
                        j.JobId,
                        j.JobName,
                        j.Description,
                        j.Salary,
                        j.Country,
                        j.Location,
                        j.PublishDate,
                        j.YearsOfExperience
                        
                    })
                    .ToListAsync();

                return Ok(filteredJobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request: " + ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutJob(Job job)
        {
            if (_context.Jobs == null)
            {
                return Problem("Entity set 'AppDbContext.Jobs' is null.");
            }
            _context.Entry(job).State = EntityState.Modified;
            // Validation for "string" values
            if (job.Location == "string" || job.Country == "string" || job.Description == "string")
            {
                return BadRequest("Location, Address, and Description fields cannot be 'string'.");
            }

            // Save changes to the database
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobExists(job.JobId))
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






    }
}
