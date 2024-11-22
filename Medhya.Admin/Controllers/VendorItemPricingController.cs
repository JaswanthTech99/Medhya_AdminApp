
using Medhya.Admin.Models;
using Medhya.Admin.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Medhya.Admin.Controllers
{
    public class VendorItemPricingController : Controller
    {
        private readonly IVendorItemPriceRepository _repository;
        public VendorItemPricingController(IVendorItemPriceRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index(int? id, int? vendorId, int? itemId,bool isCancel)
        
        
        {
            if (isCancel)
            {
                // Reset form data by returning an empty VendorItemPricing object
                var resetViewModel = new VendorItemPricingViewModel
                {
                    VendorItemPricingList = await _repository.GetVendorItemPricingListAsync(vendorId, itemId),
                    VendorItemPricing = new VendorItemPricing() // Clear the data
                };
                await ItemList();
                await ItemVendorList();
                await UomList();

                return View(resetViewModel);
            }
            var vendorItemPricing = id.HasValue
       ? await _repository.GetVendorItemPricingByIdAsync(id.Value)
       : new VendorItemPricing();
            var viewModel = new VendorItemPricingViewModel
            {
                VendorItemPricingList = await _repository.GetVendorItemPricingListAsync(vendorId, itemId),
                VendorItemPricing = vendorItemPricing // Always show an empty object for create
            };


            await ItemList();
            await ItemVendorList();
            await UomList();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendorItemPricingViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _repository.UpsertVendorItemPricingAsync(viewModel.VendorItemPricing);

                    TempData["Message"] = "Vendor item pricing saved successfully.";
                    TempData["MessageType"] = "success";
                }
                catch (Exception ex)
                {
                    TempData["Message"] = $"An error occurred: {ex.Message}";
                    TempData["MessageType"] = "error";
                }
            }
            else
            {
                TempData["Message"] = "Please ensure all fields are correctly filled.";
                TempData["MessageType"] = "error";
            }

            return RedirectToAction("Index");
        }
        public async Task<EmptyResult> ItemList()
        {
            var items = await _repository.GetItemList();
            ViewBag.ItemList = new SelectList(items, "Id", "ItemName");
            return new EmptyResult();

        }
        public async Task<EmptyResult> ItemVendorList()
        {
            var items = await _repository.GetVendorList();
            ViewBag.Vendors = new SelectList(items, "Id", "VendorName");
            return new EmptyResult();

        }
        public async Task<EmptyResult> UomList()
        {
            List<string> columnValues = await _repository.UOMList();
            ViewBag.ColumnValues = new SelectList(columnValues);
            return new EmptyResult();
        }

    }
}
