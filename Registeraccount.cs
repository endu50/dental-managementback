using System.ComponentModel.DataAnnotations;

namespace DentalDana
{
    public class Registeraccount
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }
        [Required,MinLength(6)]
        public string Password { get; set; }

        public string PhoneNumber { get; set; } // Add this
    }
}
