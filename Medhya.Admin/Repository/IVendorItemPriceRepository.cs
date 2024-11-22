using Medhya.Admin.Models;

namespace Medhya.Admin.Repository
{
    public interface IVendorItemPriceRepository
    {
        Task<int> UpsertVendorItemPricingAsync(VendorItemPricing vendorItemPricing);
        Task<VendorItemPricing> GetVendorItemPricingByIdAsync(int ?Id);
        Task<IEnumerable<VendorItemPricing>> GetVendorItemPricingListAsync(int? VendorId = null, int? ItemId = null);
        Task<IEnumerable<DDLItem>> GetItemList();
        Task<IEnumerable<DDLVendor>> GetVendorList();
        Task<List<string>> UOMList();
       // Task<VendorItemPricing> GetVendorItemPricingByIdAsync(int? id);
    }
}
