using Medhya.Admin.Models;
using System.Numerics;

namespace Medhya.Admin.Repository
{
    public interface IVendorRepository
    {
      Task<IEnumerable<Vendor>> GetVendors();
       Vendor GetVendorById(int id);
        Task<int> InsertVendor(Vendor vendor);
        //void UpdateVendor(Vendor vendor);
        
    }
}
