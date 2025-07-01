using System.Text.Json;
using System.Text;
using DentalDana;

public class AfroMessageService : ISmsSender
{
    private readonly IConfiguration _config;
    private readonly HttpClient _http;

    public AfroMessageService(IConfiguration config)
    {
        _config = config;
        _http = new HttpClient();
    }

    public async Task<bool> SendOtpAsync(string phoneNumber, string otp)
    {
        var url = "https://api.afromessage.com/api/send"; // Confirm actual endpoint

        var payload = new
        {
            to = phoneNumber,
            message = $"Your verification code is {otp}",
            sender = _config["AfroMessage:Sender"],
            api_key = _config["AfroMessage:ApiKey"]
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _http.PostAsync(url, content);
        return response.IsSuccessStatusCode;
    }
}
