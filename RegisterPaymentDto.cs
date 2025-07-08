namespace DentalDana
{
    public class RegisterPaymentDto
    {
    
        public string PaymentId { get; set; } = string.Empty;

        public string Amount { get; set; }
        public string Method { get; set; }
         public string PaymentDescription { get; set; }
        public string PaymentStatus { get; set; }
        public string PatientId { get; set; }



       
            
    }
}
