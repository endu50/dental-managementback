namespace DentalDana
{
    public class Payment
    {
        public int Id { get; set; }
        public string Method { get; set; } = "Cash"; // or "Online"
        public long Amount { get; set; }
        public DateTime datePayment { get; set; }
        public string? patientId { get; set; }

         public string PaymentDescription { get; set; }

          public string PaymentStatus { get; set; }
    }

}
