using Medhya.Admin.Models;
using Medhya.Admin.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel;
using System.Security.Claims;

namespace Medhya.Admin.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private readonly IItemRepository _itemRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ItemController(IItemRepository itemRepository, IWebHostEnvironment webHostEnvironment)
        {
            _itemRepository = itemRepository;
            _webHostEnvironment = webHostEnvironment;
        }
      
        public async Task<IActionResult> Index()
        {

            var itemList = await _itemRepository.GetAllAsync();
            // For the form

            await CategoryList();
            return View(itemList);


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
            ViewBag.ItemList = new SelectList(categories, "Id", "ItemName");
            return new EmptyResult();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Item item)
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if ((item.ImageFile == null || item.ImageFile.Length == 0))
            {
                // Add error if no file is provided during insert
                ModelState.AddModelError("ImageFile", "Image upload is mandatory for new items.");
                await CategoryList();
                return View(item); // Return with error messages
            }

            if (item.ImageFile != null && item.ImageFile.Length > 0)
            {
                var imagePath = await SaveImageAsync(item.ImageFile);
                if (imagePath == null)
                {
                    await CategoryList();
                    return View(item); // Return with validation errors
                }
                // Save the image path in the model
                item.ImagePath = imagePath;
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!ModelState.IsValid)
            {
                await CategoryList();
                return View();
            }
            await CategoryList();
            item.CreatedBy = userId;
            var result = await _itemRepository.AddAsync(item);
            if (result == 100)
            {
                TempData["Message"] = "Item details  saved successfully.";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index");
            }
            else if (result == -101)     
            {
                TempData["Message"] = "Same Item Name already exists with selected Category.";
                TempData["MessageType"] = "error";
            }
            if(result == -101)
            {
                await CategoryList();
                return View(item);
            }
            return RedirectToAction("Index");
        }
        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            // Allowed image extensions
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(imageFile.FileName);

            // Validate file extension
            if (!allowedExtensions.Contains(extension.ToLower()))
            {
                ModelState.AddModelError("ImageFile", "Only image files (.jpg, .jpeg, .png, .gif) are allowed.");
                return null;
            }

            // Validate file size (limit to 2 MB)
            if (imageFile.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("ImageFile", "File size must not exceed 2 MB.");
                return null;
            }

            try
            {
                // Save the file to the server
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + extension; // Generate unique file name
                string filePath = Path.Combine(wwwRootPath + "/images/", fileName);

                // Ensure the images directory exists
                Directory.CreateDirectory(Path.Combine(wwwRootPath, "images"));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Return the relative path to be saved in the database
                return "/images/" + fileName;
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors during file saving
                ModelState.AddModelError("ImageFile", $"An error occurred while saving the file: {ex.Message}");
                return null;
            }
        }
        public async Task<IActionResult> Edit(int Id)
        {
            var categories = await _itemRepository.CategoryList();
            var item = await _itemRepository.GetByIdAsync(Id);
            ViewBag.categoryList = new SelectList(categories, "Id", "CategoryName", item.FK_CategoryId);
            return View(item);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Item item)
        {

            if (item.ImageFile != null && item.ImageFile.Length > 0)
            {
                var newImagePath = await SaveImageAsync(item.ImageFile);
                if (newImagePath == null)
                {
                    await CategoryList();
                    return View(item); // Return with error messages
                }
                await DeleteOldImageAsync(item.ImagePath);
                item.ImagePath = newImagePath;
            }
            else
            {
                // Retain the old image path if no new image is uploaded
                item.ImagePath = item.ImagePath;
            }
            var result = _itemRepository.AddAsync(item);


            return RedirectToAction("Index");
        }
        private async Task DeleteOldImageAsync(string existingImagePath)
        {
            if (!string.IsNullOrEmpty(existingImagePath))
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string oldImagePath = Path.Combine(wwwRootPath, existingImagePath.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath); // Delete the old file
                }
            }
        }
        #region:ItemPriceByUOM
        //public async Task<IActionResult> CreateItemPrice()
        //{

        //    List<string> columnValues = await _itemRepository.UOMList();
        //    ViewBag.ColumnValues = columnValues.Select(value => new SelectListItem
        //    {
        //        Text = value,  // Display text
        //        Value = value  // Value attribute
        //    })
        //    .ToList(); ;
        //    var items = await _itemRepository.GetItemList();
        //    ViewBag.ItemList = new SelectList(items, "Id", "ItemName");
        //    return View();
        //}
        [HttpPost]
        public async Task<IActionResult> CreateItemPrice(ItemPricebyUOMViewModel itemPricebyUOMViewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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
                itemPricebyUOMViewModel.itemPriceByUOM.CreatedBy = userId;
                var result = await _itemRepository.AddItemPriceByUOM(itemPricebyUOMViewModel.itemPriceByUOM);
                if (result == 100)
                {
                    TempData["Message"] = "Item pricing saved successfully.";
                    TempData["MessageType"] = "success";
                    return RedirectToAction("ItemPriceByUOmList");
                }else if(result==-101){
                    TempData["Message"] = "A price already exists for the selected item and UOM. Please update it or choose a different UOM.";
                    TempData["MessageType"] = "error";
                }
                

            }
            List<string> columnValues = await _itemRepository.UOMList();
            ViewBag.ColumnValues = columnValues.Select(value => new SelectListItem
            {
                Text = value,  // Display text
                Value = value  // Value attribute
            });
            itemPricebyUOMViewModel.itemPriceByUOMList = await _itemRepository.ItemPriceByUOMAsyncList();
            return View("ItemPriceByUOmList", itemPricebyUOMViewModel);

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
            await ItemList();
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
                itemPriceByUOMList = await _itemRepository.ItemPriceByUOMAsyncList(),
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
