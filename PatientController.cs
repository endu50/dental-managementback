using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DentalDana
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly DentalContext _context;

        public PatientController(DentalContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
        {
            return await _context.Patients.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> AddPatient(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPatients), new { id = patient.Id }, patient);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> getPatientById(int id)
        {
            var patients = await _context.Patients.FindAsync(id);

            if(patients==null)
            {
                return NotFound();
            }

            return Ok(patients);

        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Patient>> UpdatePatient(int id, [FromBody] Patient patient)
        {
            if (id != patient.Id)
            {
                return BadRequest("Id Mismatch");
            }
            var existingPatient = await _context.Patients.FindAsync(id);
            if (existingPatient == null)
            {
                return NotFound("Patient Id not found");
            }
            existingPatient.Id = patient.Id;
            existingPatient.FullName = patient.FullName;
            existingPatient.email = patient.email;
            existingPatient.Phone = patient.Phone;
            existingPatient.Gender = patient.Gender;
            existingPatient.DateOfBirth = patient.DateOfBirth;
            try
            {
                await _context.SaveChangesAsync();
                return Ok(existingPatient);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server errror:" + ex.Message);

            }


        }

        [HttpDelete("{id}")]

        public  async Task<ActionResult> DeletePatient(int id)
        {
            var patints = await _context.Patients.FindAsync(id);
            if(patints == null)
            {
                return NotFound("Id Not found");
            }

            _context.Patients.Remove(patints);
            await _context.SaveChangesAsync();
            return NoContent();
        }



    }
}
