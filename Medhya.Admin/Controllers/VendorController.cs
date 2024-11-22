using Medhya.Admin.Models;
using Medhya.Admin.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Medhya.Admin.Controllers
{
    public class VendorController : Controller
    {
        private readonly IVendorRepository _vendorRepository;

        public VendorController(IVendorRepository vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }
        public async Task<IActionResult> Index()
        {
            var viewModel = new VendorViewModel
            {
                VendorList = await _vendorRepository.GetVendors(),
                Vendor = new Vendor() // For the form
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendorViewModel viewModel)
        {
            if (ModelState.IsValid)
            {

                // Create new vendor
                var result = await _vendorRepository.InsertVendor(viewModel.Vendor);
                if (result == 100)
                {
                    TempData["Message"] = "Vendor saved successfully";
                    TempData["MessageType"] = "success";
                }
                else if(result ==-100)
                {
                    TempData["Message"] = "Vendor saved successfully";
                    TempData["MessageType"] = "failed";
                }
                else if (result == -99)
                {
                    TempData["Message"] = "Vendor already exists";
                    TempData["MessageType"] = "exists";
                }
                return RedirectToAction("Index");
            }
            viewModel.VendorList = await _vendorRepository.GetVendors();
            return View("Index", viewModel);

        }
        public async Task<IActionResult> Edit(int id)
        {
            var vendor = _vendorRepository.GetVendorById(id);
            var viewModel = new VendorViewModel
            {
                VendorList = await _vendorRepository.GetVendors(),
                Vendor = vendor
            };

            return View("Index", viewModel);
        }
    }
}
