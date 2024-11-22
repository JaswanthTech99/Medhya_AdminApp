using System.ComponentModel.DataAnnotations;

namespace Medhya.Admin.Models
{
    public class Vendor
    {

        public int Id { get; set; }
        //public string VendorCode { get; set; }
        [Required]
        public string? VendorName { get; set; }
        [Required]
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        [Required]
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Status { get; set; }
    }
    public class VendorViewModel
    {
        public IEnumerable<Vendor>? VendorList { get; set; }
        public Vendor? Vendor { get; set; } // For Create and Update operations
    }



}
