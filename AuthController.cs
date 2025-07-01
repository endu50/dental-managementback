using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using DentalDana.Migrations;
using System.Net.Mail;
using System.Net;


namespace DentalDana 
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly DentalContext _context;
        private readonly IConfiguration _config;
        private readonly ISmsSender _smsSender;

        public AuthController(DentalContext context, IConfiguration config, ISmsSender smsSender)
        {
            _context = context;
            _config = config;
            _smsSender = smsSender;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Registeraccount dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already exists");

            CreatePasswordHash(dto.Password, out byte[] hash, out byte[] salt);

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registered" });
        }

        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<User>>> getAccount()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpDelete ("{id}")]

        public async Task<ActionResult> deleteAccount(int id)
        {
           var user= await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Id Not found");
            }

            _context.Users.Remove(user);
           await _context.SaveChangesAsync();
            return Ok(user);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(login dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt))
                return Unauthorized("Invalid credentials");

            string token = CreateToken(user);
            return Ok(new { token });
        }

        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPassword(string password, byte[] hash, byte[] salt)
        {
            using var hmac = new HMACSHA512(salt);
            var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computed.SequenceEqual(hash);
        }

        private string CreateToken(User user)
        {
            var claims = new[] {
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("request-reset")]
        public async Task<IActionResult> RequestReset([FromBody] ResetRequestDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return NotFound("User not found");

            // Generate secure token
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            user.PasswordResetToken = token;
            user.ResetTokenExpires = DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();

            // Construct reset link
            var resetLink = $"http://localhost:4200/reset-password?token={token}";

            try
            {
                // Set up SMTP client (use App Password for Gmail)
                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("endu2012@gmail.com", "iauh sbxl rdej ilqs"), // 👈 Use Gmail App Password
                    EnableSsl = true
                };

                // Create email message
                var mailMessage = new MailMessage("endu2012@gmail.com", user.Email)
                {
                    Subject = "Password Reset",
                    Body = $"Hello {user.FullName},\n\nClick the link below to reset your password:\n{resetLink}\n\nThis link will expire in 1 hour.",
                    IsBodyHtml = false
                };

                // Send the email
                await smtp.SendMailAsync(mailMessage);
                return Ok("Reset link sent.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Email send failed: {ex.Message}");
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.PasswordResetToken == dto.Token &&
                u.ResetTokenExpires > DateTime.UtcNow);

            if (user == null)
                return BadRequest("Invalid or expired token");

            CreatePasswordHash(dto.NewPassword, out byte[] hash, out byte[] salt);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password reset successful" });

        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] OtpRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (user == null)
                return NotFound("Phone number not registered");

            var otp = new Random().Next(100000, 999999).ToString();
            user.OtpCode = otp;
            user.OtpExpiresAt = DateTime.UtcNow.AddMinutes(5);
            await _context.SaveChangesAsync();

            var sent = await _smsSender.SendOtpAsync(request.PhoneNumber, otp);
            if (!sent) return StatusCode(500, "Failed to send OTP");

            return Ok("OTP sent successfully");
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.PhoneNumber == dto.PhoneNumber &&
                u.OtpCode == dto.Otp &&
                u.OtpExpiresAt > DateTime.UtcNow);

            if (user == null)
                return BadRequest("Invalid or expired OTP");

            user.OtpCode = null;
            user.OtpExpiresAt = null;
            await _context.SaveChangesAsync();

            // Generate JWT or session here if needed
            return Ok("OTP verified. Authentication successful.");
        }




    }
}
