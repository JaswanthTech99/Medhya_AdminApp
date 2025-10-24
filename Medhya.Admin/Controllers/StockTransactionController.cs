using Medhya.Admin.Models;
using Medhya.Admin.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Medhya.Admin.Controllers
{
    [Authorize]
    public class StockTransactionController : Controller
    {
        private readonly IStockTransactionsRepository _repository;
        public StockTransactionController(IStockTransactionsRepository repository)
        {
            _repository = repository;
        }
        public async Task<IActionResult> Index(int? id, bool isCancel)
        {
            if (isCancel)
            {
                var resetModel = new StockTransactionViewModel
                {
                    Transactions = await _repository.GetAllTransactionsAsync()
                };
                await ItemList();
                await ItemVendorList();
                await UomList();
                //var transactions = await _repository.GetAllTransactionsAsync();
                return View(resetModel); // Pass data to the Razor view
            } var stockTransaction = id.HasValue
                ? await _repository.GetTransactionByIdAsync(id.Value)
                : new StockTransactions();
            var viewModel = new StockTransactionViewModel
            {
                NewTransaction = stockTransaction,
                Transactions = await _repository.GetAllTransactionsAsync()
            };
            await ItemList();
            await ItemVendorList();
            await UomList();
            return View(viewModel);

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

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId

       
            transaction.Transactions = await _repository.GetAllTransactionsAsync();
            transaction.NewTransaction.CreatedBy = userId;
            if (ModelState.IsValid)
            {
                await _repository.CreateTransactionAsync(transaction.NewTransaction);
                return RedirectToAction(nameof(Index)); // Redirect to Index after creation
            }

            await ItemList();
            await ItemVendorList();
            await UomList();
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
