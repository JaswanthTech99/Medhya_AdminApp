using Medhya.API.Interfaces;
using Medhya.API.Model;

namespace Medhya.API.Services
{
    public class SMSService : ISMSService
    {
        private static Dictionary<string, int> userValidationCodes = new Dictionary<string, int>();

        public async Task<bool> SendSMSAsync(string mobileNumber,string username)
        {
            //generating four digit number
            Random random = new Random();
            int validationCode = random.Next(1000, 10000);
            userValidationCodes[username] = validationCode;
            string url = "https://enterprise.smsgupshup.com/GatewayAPI/rest?msg=OTP for Login Transaction on Medhya Naturals is "+ validationCode + " and valid till 5 minutes. Do not share this OTP to anyone for security reasons.&v=1.1&userid=2000236614&password=Srinivas@45&send_to="+mobileNumber+"&msg_type=text&method=sendMessage";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return false;
                }
            }
        }

        public async Task<bool> VerfiySMSAsync(int enteredCode, string username)
        {
            if (userValidationCodes.TryGetValue(username, out int storedCode))
            {
                return enteredCode == storedCode;
            }
            return false;
        }
    }
}
