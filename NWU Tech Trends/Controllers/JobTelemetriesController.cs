using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NWU_Tech_Trends.Models;

namespace NWU_Tech_Trends.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class JobTelemetriesController : ControllerBase
    {
        private readonly Alonempitula37460366Context _context;

        public JobTelemetriesController(Alonempitula37460366Context context)
        {
            _context = context;
        }

        // GET: api/JobTelemetries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobTelemetry>>> GetJobTelemetries()
        {
            return await _context.JobTelemetries.ToListAsync();
        }

        // GET: api/JobTelemetries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JobTelemetry>> GetJobTelemetry(int id)
        {
            var jobTelemetry = await _context.JobTelemetries.FindAsync(id);

            if (jobTelemetry == null)
            {
                return NotFound();
            }
           
            return jobTelemetry;
        }

        // PUT: api/JobTelemetries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJobTelemetry(int id, JobTelemetry jobTelemetry)
        {
            if (id != jobTelemetry.Id)
            {
                return BadRequest();
            }

            _context.Entry(jobTelemetry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobTelemetryExists(id))
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

        // POST: api/JobTelemetries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<JobTelemetry>> PostJobTelemetry(JobTelemetry jobTelemetry)
        {
            _context.JobTelemetries.Add(jobTelemetry);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJobTelemetry", new { id = jobTelemetry.Id }, jobTelemetry);
        }

        // DELETE: api/JobTelemetries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobTelemetry(int id)
        {
            var jobTelemetry = await _context.JobTelemetries.FindAsync(id);
            if (jobTelemetry == null || !JobTelemetryExists(id))
            {
                return NotFound();
            }

            _context.JobTelemetries.Remove(jobTelemetry);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool JobTelemetryExists(int id)
        {
            return _context.JobTelemetries.Any(e => e.Id == id);
        }

        [HttpGet("GetSavings/ByProject")]
        public IActionResult GetSavingsByProject(Guid projectId, DateTime startDate, DateTime endDate)
        {
            var jobTelemetries = _context.JobTelemetries
                .Where(jt => _context.Processes
                    .Any(p => p.ProcessId.ToString() == jt.ProccesId && p.ProjectId == projectId)
                    && jt.EntryDate >= startDate
                    && jt.EntryDate <= endDate)
                .ToList();

            var totalTimeSaved = jobTelemetries.Sum(jt => jt.HumanTime ?? 0);
            // Assuming you have a field for CostSaved
            //var totalCostSaved = jobTelemetries.Sum(jt => jt.CostSaved ?? 0);

            var result = new
            {
                TotalTimeSaved = totalTimeSaved,
                // TotalCostSaved = totalCostSaved
            };

            return Ok(result);
        }
        [HttpGet("GetSavings/ByClient")]
        public IActionResult GetSavingsByClient(Guid clientId, DateTime startDate, DateTime endDate)
        {
            var jobTelemetries = _context.JobTelemetries
                .Where(jt => _context.Processes
                    .Any(p => p.ProcessId.ToString() == jt.ProccesId &&
                              _context.Projects.Any(proj => proj.ProjectId == p.ProjectId && proj.ClientId == clientId))
                    && jt.EntryDate >= startDate
                    && jt.EntryDate <= endDate)
                .ToList();

            var totalTimeSaved = jobTelemetries.Sum(jt => jt.HumanTime ?? 0);
            // Assuming you have a field for CostSaved
            //var totalCostSaved = jobTelemetries.Sum(jt => jt.CostSaved ?? 0);

            var result = new
            {
                TotalTimeSaved = totalTimeSaved,
                //TotalCostSaved = totalCostSaved
            };

            return Ok(result);
        }
    }
}
