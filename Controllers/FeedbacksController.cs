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
using Arshdny_Project.Controllers;

namespace Arshdny_ProjectV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private readonly AppDbContextt _context;

        public FeedbacksController(AppDbContextt context)
        {
            _context = context;
        }

        // GET: api/Feedbacks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacks()
        {
            if (_context.Feedbacks == null)
            {
                return NotFound();
            }

            var feedbacks = await _context.Feedbacks.OrderBy(f => f.Rating).ToListAsync();
            return feedbacks;
        }


        // GET: api/Feedbacks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Feedback>> GetFeedback(int id)
        {
          if (_context.Feedbacks == null)
          {
              return NotFound();
          }
            var feedback = await _context.Feedbacks.FindAsync(id);

            if (feedback == null)
            {
                return NotFound();
            }

            return feedback;
        }

        // PUT: api/Feedbacks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutFeedback(Feedback feedback)
        {
            

            // Validate input fields
            if (string.IsNullOrEmpty(feedback.Message) || string.IsNullOrWhiteSpace(feedback.Message) || feedback.Message == "string")
            {
                return BadRequest("Message cannot be 'string' or null.");
            }

            _context.Entry(feedback).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedbackExists(feedback.FeedbackID))
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
        

        // POST: api/Feedbacks
        [HttpPost]
        public async Task<ActionResult<Feedback>> PostFeedback(FeedbackDto feedbackDto)
        {
            if (_context.Feedbacks == null)
            {
                return Problem("Entity set 'AppDbContext.Feedbacks' is null.");
            }
            

            // Validate input fields
            if (string.IsNullOrEmpty(feedbackDto.Message) || string.IsNullOrWhiteSpace(feedbackDto.Message) || feedbackDto.Message == "string")
            {
                return BadRequest("Message cannot be 'string' or null.");
            }

            // Map FeedbackDto to Feedback
            var feedback = new Feedback
            {
                RefugeeID= feedbackDto.RefugeeID,
                Rating = feedbackDto.Rating,
                Message = feedbackDto.Message,
                FeedbackDate=feedbackDto.FeedbackDate

                // Map other necessary properties here
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            // Return the created feedback with FeedbackID in the response
            return CreatedAtAction("GetFeedback", new { id = feedback.FeedbackID }, feedback);
        }


        //// DELETE: api/Feedbacks/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteFeedback(int id)
        //{
        //    if (_context.Feedbacks == null)
        //    {
        //        return NotFound();
        //    }
        //    var feedback = await _context.Feedbacks.FindAsync(id);
        //    if (feedback == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Feedbacks.Remove(feedback);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool FeedbackExists(int id)
        {
            return (_context.Feedbacks?.Any(e => e.FeedbackID == id)).GetValueOrDefault();
        }
    }
}
