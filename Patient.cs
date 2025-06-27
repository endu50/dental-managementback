using Microsoft.AspNetCore.Identity;

namespace DentalDana
{
    public class Patient
    {
        public int Id { get; set; }
        public  string FullName { get; set; }
        public string email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
