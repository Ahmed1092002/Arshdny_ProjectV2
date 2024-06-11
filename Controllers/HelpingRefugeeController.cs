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

namespace Arshdny_ProjectV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpingRefugeeController : ControllerBase
    {
        private readonly AppDbContextt _context;

        public HelpingRefugeeController(AppDbContextt context)
        {
            _context = context;
        }

        // GET: api/RefugeeHelps
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HelpingRefugee>>> GetRefugeeHelps()
        {
          if (_context.HelpingRefugees == null)
          {
              return NotFound();
          }
            return await _context.HelpingRefugees.ToListAsync();
        }

        // GET: api/RefugeeHelps/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HelpingRefugee>> GetRefugeeHelp(int id)
        {
          if (_context.HelpingRefugees == null)
          {
              return NotFound();
          }
            var refugeeHelp = await _context.HelpingRefugees.FindAsync(id);

            if (refugeeHelp == null)
            {
                return NotFound();
            }

            return refugeeHelp;
        }

        // PUT: api/RefugeeHelps/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutRefugeeHelp( HelpingRefugee refugeeHelp)
        {

            _context.Entry(refugeeHelp).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RefugeeHelpExists(refugeeHelp.RequestID))
                {
                    return NotFound();
                }
                else
                {
                    return CreatedAtAction("GetRefugeeHelp", new { id = refugeeHelp.RequestID }, refugeeHelp);
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<HelpingRefugee>> PostRefugeeHelp(HelpingRefugeeDto refugeeHelpDto)
        {
            if (_context.HelpingRefugees == null)
            {
                return Problem("Entity set 'AppDbContext.HelpingRefugees' is null.");
            }

            // Validate input fields
            if (refugeeHelpDto.Message == "string" || string.IsNullOrEmpty(refugeeHelpDto.Message) || string.IsNullOrWhiteSpace(refugeeHelpDto.Message))
            {
                return BadRequest("Message cannot be 'string' or null.");
            }

            var refugeeHelp = new HelpingRefugee
            {
                RefugeeID = refugeeHelpDto.RefugeeID,
                Message = refugeeHelpDto.Message,
                RequestDate = refugeeHelpDto.RequestDate
            };

            _context.HelpingRefugees.Add(refugeeHelp);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRefugeeHelp", new { id = refugeeHelp.RequestID }, refugeeHelp);
        }


        // DELETE: api/RefugeeHelps/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRefugeeHelp(int id)
        {
            if (_context.HelpingRefugees == null)
            {
                return NotFound();
            }
            var refugeeHelp = await _context.HelpingRefugees.FindAsync(id);
            if (refugeeHelp == null)
            {
                return NotFound();
            }

            _context.HelpingRefugees.Remove(refugeeHelp);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RefugeeHelpExists(int id)
        {
            return (_context.HelpingRefugees?.Any(e => e.RequestID == id)).GetValueOrDefault();
        }


        [HttpPut("EditRequestStatus/{RequestID}")]
        public async Task<IActionResult> EditRequestStatus(int RequestID, bool RequestStatus)
        {
            // Check if the job exists
            var Request = await _context.HelpingRefugees.FindAsync(RequestID);
            if (Request == null)
            {
                return NotFound();
            }

            // Update the IsView property
            Request.RequestStatus = RequestStatus;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RefugeeHelpExists(RequestID))
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
