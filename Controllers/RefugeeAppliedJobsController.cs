using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Arshdny_ProjectV2.AppDbContext;
using Arshdny_ProjectV2.Models;
using System.Security.Cryptography;
using Arshdny_ProjectV2.DtoModels;

namespace Arshdny_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefugeeAppliedJobsController : ControllerBase
    {
        private readonly AppDbContextt _context;
        public enum JobStatus
        {
            Pending = 0,
            Accepted = 1,
            Rejected = 2
        }
        public RefugeeAppliedJobsController(AppDbContextt context)
        {
            _context = context;
        }

        // GET: api/RefugeeAppliedJobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RefugeeAppliedJob>>> GetRefugeeAppliedJobs()
        {
          if (_context.RefugeeAppliedJobs == null)
          {
              return NotFound();
          }
            return await _context.RefugeeAppliedJobs.ToListAsync();
        }

        // GET: api/RefugeeAppliedJobs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RefugeeAppliedJob>> GetRefugeeAppliedJob(int id)
        {
          if (_context.RefugeeAppliedJobs == null)
          {
              return NotFound();
          }
            var refugeeAppliedJob = await _context.RefugeeAppliedJobs.Where(d => d.RefugeeId == id).ToListAsync();

            if (refugeeAppliedJob == null)
            {
                return NotFound();
            }

            return Ok(refugeeAppliedJob);
        }


        // POST: api/RefugeeAppliedJobs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RefugeeAppliedJob>> PostRefugeeAppliedJob(RefugeeAppliedJobDto refugeeAppliedJobDto)
        {
            var refugeeAppliedJob = new RefugeeAppliedJob
            {
                RefugeeId = refugeeAppliedJobDto.RefugeeId,
                JobId = refugeeAppliedJobDto.JobId,
                ApplyDate = refugeeAppliedJobDto.ApplyDate,
                JobStatus = 0 // Assuming a default status when creating new entry
            };

            if (_context.RefugeeAppliedJobs == null)
            {
                return Problem("Entity set 'AppDbContext.RefugeeAppliedJobs' is null.");
            }

            var existingJob = await _context.RefugeeAppliedJobs
                .FirstOrDefaultAsync(ra => ra.RefugeeId == refugeeAppliedJob.RefugeeId && ra.JobId == refugeeAppliedJob.JobId);

            if (existingJob != null)
            {
                if (existingJob.JobStatus == Convert.ToInt32(JobStatus.Rejected))
                {
                    _context.RefugeeAppliedJobs.Remove(existingJob);
                    refugeeAppliedJob.JobStatus = 0; // Reset to pending status
                }
                if(existingJob.JobStatus == Convert.ToInt32(JobStatus.Pending) || existingJob.JobStatus == Convert.ToInt32(JobStatus.Accepted))
                {
                    return BadRequest("Refugee Alredy Applied on this Job");
                }
               
            }
         



            _context.RefugeeAppliedJobs.Add(refugeeAppliedJob);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RefugeeAppliedJobExists(refugeeAppliedJob.RefugeeId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            

            return CreatedAtAction("GetRefugeeAppliedJob", new { id = refugeeAppliedJob.RefugeeId }, refugeeAppliedJob);
        }


        // DELETE: api/RefugeeAppliedJobs/5/10
        [HttpDelete("{RefugeeID}/{JobID}")]
        public async Task<IActionResult> DeleteRefugeeAppliedJob(int RefugeeID, int JobID)
        {
            if (_context.RefugeeAppliedJobs == null)
            {
                return NotFound();
            }

            var refugeeAppliedJob = await _context.RefugeeAppliedJobs
                .FirstOrDefaultAsync(d => d.RefugeeId == RefugeeID && d.JobId == JobID);

            if (refugeeAppliedJob == null)
            {
                return NotFound();
            }

            _context.RefugeeAppliedJobs.Remove(refugeeAppliedJob);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        //// GET: api/RefugeeAppliedJobs/IsPending
        //[HttpGet("IsPending")]
        //public async Task<ActionResult<bool>> IsPending(int refugeeId, int jobId)
        //{
        //    // Check if the refugee exists
        //    var refugeeExists = await _context.RefugeeAppliedJobs.AnyAsync(r => r.RefugeeId == refugeeId);
        //    if (!refugeeExists)
        //    {
        //        return NotFound("Refugee ID not found.");
        //    }

        //    // Check if the job exists
        //    var jobExists = await _context.RefugeeAppliedJobs.AnyAsync(j => j.JobId == jobId);
        //    if (!jobExists)
        //    {
        //        return NotFound("Job ID not found.");
        //    }

        //    // Check if there's a pending application for the specified refugee and job
        //    var isPending = await _context.RefugeeAppliedJobs.AnyAsync(r => r.RefugeeId == refugeeId && r.JobId == jobId && r.JobStatus == 0);

        //    return isPending;
        //}

        // GET: api/RefugeeAppliedJobs/IsAccepted


        //[HttpGet("IsAccepted")]
        //public async Task<ActionResult<bool>> IsAccepted(int refugeeId, int jobId)
        //{
        //    // Check if the refugee exists
        //    var refugeeExists = await _context.RefugeeAppliedJobs.AnyAsync(r => r.RefugeeId == refugeeId);
        //    if (!refugeeExists)
        //    {
        //        return NotFound("Refugee ID not found.");
        //    }

        //    // Check if the job exists
        //    var jobExists = await _context.RefugeeAppliedJobs.AnyAsync(j => j.JobId == jobId);
        //    if (!jobExists)
        //    {
        //        return NotFound("Job ID not found.");
        //    }

        //    // Check if there's a pending application for the specified refugee and job
        //    var IsAccepted = await _context.RefugeeAppliedJobs.AnyAsync(r => r.RefugeeId == refugeeId && r.JobId == jobId && r.JobStatus == 1);

        //    return IsAccepted;
        //}

        // GET: api/RefugeeAppliedJobs/IsRejected


        //[HttpGet("IsRejected")]
        //public async Task<ActionResult<bool>> IsRejected(int refugeeId, int jobId)
        //{
        //    // Check if the refugee exists
        //    var refugeeExists = await _context.RefugeeAppliedJobs.AnyAsync(r => r.RefugeeId == refugeeId);
        //    if (!refugeeExists)
        //    {
        //        return NotFound("Refugee ID not found.");
        //    }

        //    // Check if the job exists
        //    var jobExists = await _context.RefugeeAppliedJobs.AnyAsync(j => j.JobId == jobId);
        //    if (!jobExists)
        //    {
        //        return NotFound("Job ID not found.");
        //    }

        //    // Check if there's a pending application for the specified refugee and job
        //    var IsRejected = await _context.RefugeeAppliedJobs.AnyAsync(r => r.RefugeeId == refugeeId && r.JobId == jobId && r.JobStatus == 2);

        //    return IsRejected;
        //}

        private bool RefugeeAppliedJobExists(int id)
        {
            return (_context.RefugeeAppliedJobs?.Any(e => e.RefugeeId == id)).GetValueOrDefault();
        }


        [HttpGet("CheckRefugeeAppliedForJob")]
        public async Task<ActionResult<bool>> CheckRefugeeAppliedForJob(int refugeeId, int jobId)
        {
            try
            {
                // Check if there is an application with the given refugeeId and jobId
                var applicationExists = await _context.RefugeeAppliedJobs
                    .AnyAsync(app => app.RefugeeId == refugeeId && app.JobId == jobId);

                return Ok(applicationExists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request: " + ex.Message);
            }
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

        // GET: api/RefugeeAppliedJobs/CheckIfApplied
        [HttpGet("CheckIfApplied")]
        public async Task<ActionResult<bool>> CheckIfApplied(int refugeeId, int jobId)
        {
            // Check if the refugee exists
            var refugeeExists = await _context.Refugees.AnyAsync(r => r.RefugeeId == refugeeId);
            if (!refugeeExists)
            {
                return NotFound("Refugee ID not found.");
            }

            // Check if the job exists
            var jobExists = await _context.Jobs.AnyAsync(j => j.JobId == jobId);
            if (!jobExists)
            {
                return NotFound("Job ID not found.");
            }

            // Check if there's an application with JobStatus 0 or 1 for the specified refugee and job
            var applied = await _context.RefugeeAppliedJobs
                .AnyAsync(r => r.RefugeeId == refugeeId && r.JobId == jobId && (r.JobStatus == 0 || r.JobStatus == 1));

            return applied;
        }

        [HttpPut]
        public async Task<IActionResult> EditRefugeeAppliedJobStatus(int refugeeId, int jobId, int jobStatus)
        {
            if (refugeeId == null || jobId == null)
            {
                return BadRequest("RefugeeId and JobId must be provided.");
            }

            // Check if the refugee exists
            var refugee = await _context.Refugees.FirstOrDefaultAsync(r => r.RefugeeId == refugeeId);
            if (refugee == null)
            {
                return NotFound($"Refugee with ID {refugeeId} not found.");
            }

            // Check if the job exists
            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId == jobId);
            if (job == null)
            {
                return NotFound($"Job with ID {jobId} not found.");
            }

            // Check if the refugee applied job exists
            var refugeeAppliedJob = await _context.RefugeeAppliedJobs.FirstOrDefaultAsync(r => r.RefugeeId == refugeeId && r.JobId == jobId);
            if (refugeeAppliedJob == null)
            {
                return NotFound($"Refugee with ID {refugeeId} has not applied for Job with ID {jobId}.");
            }

            // Update the job status
            refugeeAppliedJob.JobStatus = jobStatus;

            _context.Entry(refugeeAppliedJob).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RefugeeAppliedJobExists(refugeeId))
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


        // GET: api/Refugees/GetAllJobDetailsByRefugeeID/27
        [HttpGet("GetAllJobDetailsByRefugeeID/{refugeeId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllJobDetailsByRefugeeID(int refugeeId)
        {
            if (_context.Refugees == null || _context.RefugeeAppliedJobs == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var jobDetails = await (from r in _context.Refugees
                                    join raj in _context.RefugeeAppliedJobs on r.RefugeeId equals raj.RefugeeId
                                    join j in _context.Jobs on raj.JobId equals j.JobId
                                    where r.RefugeeId == refugeeId
                                    select new
                                    {
                                        j.JobId,
                                        j.JobName,
                                        j.Description,
                                        j.Salary,
                                        raj.JobStatus,
                                        r.RefugeeId,
                                        r.UserId,
                                        r.Cv,
                                        r.CountryId,
                                        r.ImagePath
                                    }).ToListAsync();

            if (jobDetails == null || !jobDetails.Any())
            {
                return NotFound();
            }

            return Ok(jobDetails);
        }

    }
}
