using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Medhya.Admin.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private readonly IItemRepository _itemRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        public ItemController(IItemRepository itemRepository, IWebHostEnvironment webHostEnvironment, IConfiguration config)
        {
            _itemRepository = itemRepository;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
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
            if (result == -101)
            {
                await CategoryList();
                return View(item);
            }
            return RedirectToAction("Index");
        }


        private async Task<string?> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile is null || imageFile.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "Empty file.");
                return null;
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(imageFile.FileName)?.ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(extension) || !allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("ImageFile", "Only image files (.jpg, .jpeg, .png, .gif, .webp) are allowed.");
                return null;
            }

            if (imageFile.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("ImageFile", "File size must not exceed 2 MB.");
                return null;
            }

            try
            {
                // Azure Blob connection info
                var conn = _config["AzureStorage:ConnectionString"];
                var containerName = _config["AzureStorage:ContainerName"]; // "uploads"

                var blobServiceClient = new BlobServiceClient(conn);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                // Generate unique file name under Items folder
                var fileName = $"Items/{Guid.NewGuid():N}{extension}";
                var blobClient = containerClient.GetBlobClient(fileName);

                // Upload to Blob Storage
                using var stream = imageFile.OpenReadStream();
                await blobClient.UploadAsync(stream, overwrite: true);

                // ✅ Return public Blob URL
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ImageFile", $"An error occurred while saving to Azure Blob: {ex.Message}");
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
            if (string.IsNullOrEmpty(existingImagePath))
                return;

            try
            {
                // Read connection and container info from configuration
                var conn = _config["AzureStorage:ConnectionString"];
                var containerName = _config["AzureStorage:ContainerName"]; // e.g. "uploads"

                var blobServiceClient = new BlobServiceClient(conn);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                // Extract the blob path
                // Handles both relative ("Items/file.png") and full URLs
                string blobPath;
                if (existingImagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    var uri = new Uri(existingImagePath);
                    blobPath = uri.AbsolutePath.TrimStart('/'); // uploads/Items/xyz.png
                }
                else
                {
                    blobPath = existingImagePath.TrimStart('/');
                }

                // Get blob client and delete it
                var blobClient = containerClient.GetBlobClient(blobPath);
                await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                // Optional: log error
                Console.WriteLine($"[Warning] Could not delete blob: {ex.Message}");
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

            // Always bind the Item dropdown (you already have this helper)
            await ItemList();

            // Always bind the UOM dropdown based on the selected item
            var selectedItemId = itemPricebyUOMViewModel.itemPriceByUOM?.FK_ItemId ?? 0;
            await BindUomsAsync(selectedItemId);

            if (!ModelState.IsValid)
            {
                // Refill the grid/list below the form
                itemPricebyUOMViewModel.itemPriceByUOMList = await _itemRepository.ItemPriceByUOMAsyncList();
                return View("ItemPriceByUOmList", itemPricebyUOMViewModel);
            }

            // Save
            itemPricebyUOMViewModel.itemPriceByUOM!.CreatedBy = userId;
            var result = await _itemRepository.AddItemPriceByUOM(itemPricebyUOMViewModel.itemPriceByUOM);

            if (result == 100)
            {
                TempData["Message"] = "Item pricing saved successfully.";
                TempData["MessageType"] = "success";
                return RedirectToAction("ItemPriceByUOmList");
            }
            else if (result == -101)
            {
                TempData["Message"] = "A price already exists for the selected item and UOM. Please update it or choose a different UOM.";
                TempData["MessageType"] = "error";
            }
            else
            {
                TempData["Message"] = "Failed to save price.";
                TempData["MessageType"] = "error";
            }

            // If we reach here, redisplay the page with the same per-item UOMs + list
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
                await BindUomsAsync(resetViewModel.itemPriceByUOM?.FK_ItemId ?? 0, resetViewModel.itemPriceByUOM?.UOM);
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
            await BindUomsAsync(viewModel.itemPriceByUOM?.FK_ItemId ?? 0, viewModel.itemPriceByUOM?.UOM);
            return View(viewModel);
        }

        private async Task BindUomsAsync(int itemId, string? selectedUom = null)
        {
            var uoms = itemId > 0
                ? await _itemRepository.GetUomsForItemAsync(itemId)
                : new List<string>();

            ViewBag.ColumnValues = uoms
                .Select(u => new SelectListItem
                {
                    Text = u,
                    Value = u,
                    Selected = !string.IsNullOrEmpty(selectedUom) &&
                               string.Equals(u, selectedUom, StringComparison.OrdinalIgnoreCase)
                })
                .ToList();
        }
        private async Task BindUomsAsync(int itemId)
        {
            var uoms = itemId > 0
                ? await _itemRepository.GetUomsForItemAsync(itemId)
                : new List<string>();  // empty until item selected (or load all if you prefer)

            ViewBag.ColumnValues = uoms.Select(v => new SelectListItem { Text = v, Value = v }).ToList();
        }
        [HttpGet("admin/itemprices/uoms")]
        public async Task<IActionResult> GetUomsForItem(int itemId)
        {
            var uoms = await _itemRepository.GetUomsForItemAsync(itemId);
            return Ok(uoms); // ["KG","Bunch","PCS", ...]
        }

        public async Task<IActionResult> ItemPriceByUomEdit(int Id)
        {
            var viewModel = new ItemPricebyUOMViewModel
            {
                itemPriceByUOM = await _itemRepository.GetItemPriceByUOMById(Id),
                itemPriceByUOMList = await _itemRepository.ItemPriceByUOMAsyncList(),
            };
            var uoms = Id > 0
         ? await _itemRepository.GetUomsForItemAsync(Id)
         : new List<string>();

            ViewBag.ColumnValues = uoms
                .Select(u => new SelectListItem
                {
                    Text = u,
                    Value = u,
                    Selected = !string.IsNullOrEmpty(viewModel.itemPriceByUOM.UOM) &&
                               string.Equals(u, viewModel.itemPriceByUOM.UOM, StringComparison.OrdinalIgnoreCase)
                })
                .ToList();
            var items = await _itemRepository.GetItemList();
            ViewBag.ItemList = new SelectList(items, "Id", "ItemName", viewModel.itemPriceByUOM.UOM);
            return View("ItemPriceByUOmList", viewModel);

        }
        #endregion
    }
}
