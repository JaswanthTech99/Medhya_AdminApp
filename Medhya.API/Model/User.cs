
using System.ComponentModel.DataAnnotations;

namespace Medhya.API.Model
{
    public class User
    {
        public User()
        {
            MobileNumber = "";
        }
        public string? Name { get; set; }
        public int UserId { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        public string OTP { get; set; }
        public bool IsMobileVerified { get; set; }


    }
}
