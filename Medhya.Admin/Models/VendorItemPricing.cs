using System.ComponentModel.DataAnnotations;

namespace Medhya.Admin.Models
{
    public class VendorItemPricing
    {

        public int Id { get; set; }
        [Required]
        public int FK_VendorID { get; set; }
        [Required]
        public int FK_ItemID { get; set; }
        [Required]
        public string? FK_UOM { get; set; }
        [Required]
        public decimal PricePerUOM { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public string? ItemName { get; set; }
        public string? VendorName {  get; set; }


    }

    public class VendorItemPricingViewModel
    {
        public IEnumerable<VendorItemPricing>? VendorItemPricingList { get; set; }
        public VendorItemPricing? VendorItemPricing { get; set; }
    }
}
