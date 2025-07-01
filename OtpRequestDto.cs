using System.ComponentModel.DataAnnotations;

namespace DentalDana
{
    public class OtpRequestDto
    {
        [Required]
        public string PhoneNumber { get; set; }
    }
}
