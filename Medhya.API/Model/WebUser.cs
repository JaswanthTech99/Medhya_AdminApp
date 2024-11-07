using System.ComponentModel.DataAnnotations;

namespace Medhya.API.Model
{
    public class WebUser
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        public string? Role { get; set; }
        public string? Token { get; set; }
        
    }
    public class WebUserResponse
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
    }
}
