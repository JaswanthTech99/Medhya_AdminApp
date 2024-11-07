using Medhya.API.Model;

namespace Medhya.API.Interfaces
{
    public interface ISMSService
    {
        Task<bool> SendSMSAsync(string mobileNumber, string userName);
        Task<bool> VerfiySMSAsync(int code,string userName);
    }
}
