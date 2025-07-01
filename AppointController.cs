using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalDana
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointController : Controller
    {
        private readonly DentalContext _context;
        public AppointController(DentalContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appoint>>> getAppointments()
        {
            return await _context.Appoints.ToListAsync();
        }

        [HttpPost]

        public async Task<ActionResult<Appoint>> Create(Appoint appoint)
        {
            _context.Appoints.Add(appoint);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(getAppointments), new { id = appoint.appointId }, appoint);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Appoint>> Get(int id)
        {
            var appointment = await _context.Appoints.FindAsync(id);
            if (appointment == null)
                return NotFound();
            return appointment;
        }

        [HttpPut("{id}")]
        public async Task< IActionResult> UpdateAppoint(int id, [FromBody] Appoint appoint)
        {
            if (id != appoint.appointId)
            {
                return BadRequest("Appointment ID mismatch.");
            }

            var existingAppoint = await _context.Appoints.FindAsync(id);
            if (existingAppoint == null)
            {
                return NotFound("Appointment not found.");
            }

            // Update properties
            existingAppoint.patientName = appoint.patientName;
            existingAppoint.appointmentDate = appoint.appointmentDate;
            existingAppoint.treatmentType = appoint.treatmentType;
            existingAppoint.dentistName = appoint.dentistName;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(existingAppoint);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appoints.FindAsync(id);
            if (appointment == null)
                return NotFound();

            _context.Appoints.Remove(appointment);
            await _context.SaveChangesAsync();
            return NoContent();
        }



    }
}
