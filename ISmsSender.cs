namespace DentalDana
{
    public interface ISmsSender
    {
        Task<bool> SendOtpAsync(string phoneNumber, string message);
    }
}
