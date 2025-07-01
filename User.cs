namespace DentalDana
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

     

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }

        public string OtpCode { get; set; }
        public DateTime? OtpExpiresAt { get; set; }

        public string PhoneNumber { get; set; }

    }
}
