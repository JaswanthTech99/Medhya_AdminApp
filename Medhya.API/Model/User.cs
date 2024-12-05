
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
        public string Email { get;set; }
        public bool IsMobileVerified { get; set; }
        public class UserAddress
        {
            public int? FK_UserId { get; set; }
            public string AddressLine1 { get; set; }
            public string Area { get; set; }
            public string State { get; set; }
            public string City { get; set; }
            public decimal? Longitude { get; set; }
            public decimal? Latitude { get; set; }
            public DateTime? CreatedDate { get; set; }
            public DateTime? UpdatedDate { get; set; }
        }

    }
}
