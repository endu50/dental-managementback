using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Data.Sql;
using Microsoft.EntityFrameworkCore;

namespace DentalDana
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly DentalContext _context;
        public PaymentController(DentalContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            return await _context.Payments.ToListAsync();
        }
        [HttpPost]
        public async Task<IActionResult> payments(Payment payments)
        {
            _context.Payments.Add(payments);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPayments), new { id = payments.Id }, payments);
        }
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPaymentsByPatientId(int patientId)
        {
            var payments = await _context.Payments
                .Where(p => p.patientId == patientId.ToString())
                .ToListAsync();

            if (payments == null || !payments.Any())
                return NotFound();

            return Ok(payments);
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateReceipt([FromBody] Payment model)
        {
            model.datePayment = DateTime.Now;
            _context.Payments.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }
    }
}
