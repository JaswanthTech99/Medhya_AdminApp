using Medhya.Admin.Models;
using Medhya.Admin.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Medhya.Admin.Controllers
{
    public class StockTransactionController : Controller
    {
        private readonly IStockTransactionsRepository _repository;
        public StockTransactionController(IStockTransactionsRepository repository)
        {
            _repository = repository;
        }
        public async Task<IActionResult> Index()
        {

            var viewModel = new StockTransactionViewModel
            {
                Transactions = await _repository.GetAllTransactionsAsync()
            };
            await ItemList();
            await ItemVendorList();
            //var transactions = await _repository.GetAllTransactionsAsync();
            return View(viewModel); // Pass data to the Razor view
        }
        public async Task<IActionResult> Details(int id)
        {
            var transaction = await _repository.GetTransactionByIdAsync(id);
            if (transaction == null)
                return NotFound();

            return View(transaction); // Pass data to the Razor view
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockTransactionViewModel transaction)
        {

            //if (ModelState.IsValid)
            //{
            //    await _repository.CreateTransactionAsync(transaction.NewTransaction);
            //    return RedirectToAction(nameof(Index));
            //}

            transaction.Transactions = await _repository.GetAllTransactionsAsync(); // Repopulate the list
            if (ModelState.IsValid)
            {
                await _repository.CreateTransactionAsync(transaction.NewTransaction);
                return RedirectToAction(nameof(Index)); // Redirect to Index after creation
            }

            await ItemList();
            await ItemVendorList();
            // return PartialView("_CreatePartial", transaction);

            transaction.Transactions = await _repository.GetAllTransactionsAsync();
            
            return View("Index", transaction);

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
            ViewBag.ItemList = new SelectList(items, "Id", "VendorName");
            return new EmptyResult();

        }
        
    }
}
