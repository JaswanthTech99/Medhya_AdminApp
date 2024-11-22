using Medhya.Admin.Models;
using Medhya.Admin.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Medhya.Admin.Controllers
{
    public class ItemController : Controller
    {
        private readonly IItemRepository _itemRepository;
        public ItemController(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }
        public async Task<IActionResult> Index()
        {
            var viewModel = new ItemViewModel
            {
                itemList = await _itemRepository.GetAllAsync(),
                item = new Item() // For the form
            };
            await CategoryList();
            return View(viewModel);


            //var items = await _itemRepository.GetAllAsync();
            //return View(items);
        }
        public async Task<IActionResult> Create()
        {
            await CategoryList();
            return View();
        }
        public async Task<EmptyResult> CategoryList()
        {
            var categories = await _itemRepository.CategoryList();
            ViewBag.categoryList = new SelectList(categories, "Id", "CategoryName");
            return new EmptyResult();
        }
        public async Task<EmptyResult> ItemList()
        {
            var categories = await _itemRepository.GetItemList();
            ViewBag.categoryList = new SelectList(categories, "Id", "ItemName");
            return new EmptyResult();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ItemViewModel itemViewModel)
        {




            if (!ModelState.IsValid)
            {
                await CategoryList();
                return View();
            }
            await CategoryList();
            var result = _itemRepository.AddAsync(itemViewModel.item);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int Id)
        {
            var categories = await _itemRepository.CategoryList();
            var item = await _itemRepository.GetByIdAsync(Id);
            var viewModel = new ItemViewModel
            {
                itemList = await _itemRepository.GetAllAsync(),
                item = item
            };
            ViewBag.categoryList = new SelectList(categories, "Id", "CategoryName", item.FK_CategoryId);
            return View("Index",  viewModel);
        }
        [HttpPost]
        //public async Task<IActionResult> Edit(ItemViewModel item)
        //{
        //    var result = _itemRepository.AddAsync(item);


        //    return RedirectToAction("Index");
        //}
        #region:ItemPriceByUOM
        public async Task<IActionResult> CreateItemPrice()
        {

            List<string> columnValues = await _itemRepository.UOMList();
            ViewBag.ColumnValues = columnValues.Select(value => new SelectListItem
            {
                Text = value,  // Display text
                Value = value  // Value attribute
            })
            .ToList(); ;
            var items = await _itemRepository.GetItemList();
            ViewBag.ItemList = new SelectList(items, "Id", "ItemName");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateItemPrice(ItemPricebyUOMViewModel itemPricebyUOMViewModel)
        {
            if (ModelState.IsValid)
            {

                List<string> uoms = await _itemRepository.UOMList();
                ViewBag.ColumnValues = uoms.Select(value => new SelectListItem
                {
                    Text = value,  // Display text
                    Value = value  // Value attribute
                })
                .ToList();
                await ItemList();         
                var result = await _itemRepository.AddItemPriceByUOM(itemPricebyUOMViewModel.itemPriceByUOM);
                 RedirectToAction("ItemPriceByUOmList");

            }
            List<string> columnValues = await _itemRepository.UOMList();
            ViewBag.ColumnValues = columnValues.Select(value => new SelectListItem
            {
                Text = value,  // Display text
                Value = value  // Value attribute
            });
            var list = await _itemRepository.ItemPriceByUOMAsyncList();
            return View("ItemPriceByUOmList",itemPricebyUOMViewModel);
            
        }
        public async Task<IActionResult> ItemPriceByUOmList(int? id, bool isCancel)

        {
            if (isCancel)
            {
                // Reset form data by returning an empty VendorItemPricing object
                var resetViewModel = new ItemPricebyUOMViewModel
                {
                    itemPriceByUOMList = await _itemRepository.ItemPriceByUOMAsyncList(),
                    itemPriceByUOM = new ItemPriceByUOM() // Clear the data
                };
                await ItemList();
                List<string> uoms = await _itemRepository.UOMList();
                ViewBag.ColumnValues = uoms.Select(value => new SelectListItem
                {
                    Text = value,  // Display text
                    Value = value  // Value attribute
                });
                return View(resetViewModel);
            }
            var itemUOMPrice = id.HasValue
               ? await _itemRepository.GetItemPriceByUOMById(id.Value)
               : new ItemPriceByUOM();
            var viewModel = new ItemPricebyUOMViewModel
            {
                itemPriceByUOM = itemUOMPrice,
                itemPriceByUOMList = await _itemRepository.ItemPriceByUOMAsyncList(),

            };
            var items = await _itemRepository.GetItemList();
            ViewBag.ItemList = new SelectList(items, "Id", "ItemName");
            List<string> columnValues = await _itemRepository.UOMList();
            ViewBag.ColumnValues = columnValues.Select(value => new SelectListItem
            {
                Text = value,  // Display text
                Value = value,  // Value attribute
            });                      
            return View(viewModel);
        }
         public async Task<IActionResult> ItemPriceByUomEdit(int Id)
        {
            var viewModel = new ItemPricebyUOMViewModel
            {
                itemPriceByUOM = await _itemRepository.GetItemPriceByUOMById(Id),
                itemPriceByUOMList =  await _itemRepository.ItemPriceByUOMAsyncList(),
            };
            List<string> columnValues = await _itemRepository.UOMList();
            ViewBag.ColumnValues = columnValues.Select(value => new SelectListItem
            {
                Text = value,  // Display text
                Value = value,
               // Value attribute
            })
            .ToList();
            var items = await _itemRepository.GetItemList();
            ViewBag.ItemList = new SelectList(items, "Id", "ItemName", viewModel.itemPriceByUOM.UOM);
            return View("ItemPriceByUOmList", viewModel);
           
        }
        #endregion
    }
}
