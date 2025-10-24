using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Medhya.Admin.Models;
using Medhya.Admin.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Medhya.Admin.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly IcategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        public CategoryController(IcategoryRepository categoryRepository, IWebHostEnvironment env, IConfiguration config)
        {
            _categoryRepository = categoryRepository;
            _env = env;
            _config = config;
        }

        // ---------- LIST ----------
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllCategories();
            return View(categories);
        }

        // ---------- CREATE / EDIT (GET) ----------
        // /Category/Create          -> create mode
        // /Category/Create/5        -> edit mode (loads existing)
        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null || id == 0)
            {
                // New category (Create)
                return View(new Category());
            }

            var category = await _categoryRepository.GetCategoryById(id.Value);
            if (category == null) return NotFound();

            // Reuse the same view; it will render preview + Remove checkbox when ImagePath is present
            return View(category);
        }

        // ---------- CREATE / EDIT (POST) ----------
        // Same action handles both insert & update based on Id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            // On EDIT, image is optional → don't require ImageFile
            if (model.Id > 0)
                ModelState.Remove(nameof(Category.ImageFile));

            if (!ModelState.IsValid)
                return View(model);
            model.CategoryStatus = (model.CategoryStatus == "I") ? "I" : "A";
            // If user ticked RemoveImage (in edit) -> clear existing file
            if (model.Id > 0 && model.RemoveImage && !string.IsNullOrWhiteSpace(model.ImagePath))
            {
                DeletePhysicalFile(model.ImagePath);
                model.ImagePath = null;
            }

            // If a new file is uploaded -> save & replace
            if (model.ImageFile is not null && model.ImageFile.Length > 0)
            {
                var save = await SaveCategoryImageAsync(model.ImageFile);
                if (!save.ok)
                {
                    ModelState.AddModelError(nameof(model.ImageFile), save.error!);
                    return View(model);
                }

                // If editing and there was a previous image different from new one, delete old
                if (model.Id > 0 && !string.IsNullOrWhiteSpace(model.ImagePath) && !string.Equals(model.ImagePath, save.path, StringComparison.OrdinalIgnoreCase))
                {
                    DeletePhysicalFile(model.ImagePath!);
                }

                model.ImagePath = save.path;
            }
            else
            {
                // CREATE requires an image
                if (model.Id == 0 && string.IsNullOrWhiteSpace(model.ImagePath))
                {
                    ModelState.AddModelError(nameof(model.ImageFile), "Please upload an image.");
                    return View(model);
                }
                // EDIT with no new file: keep current ImagePath as-is
            }

            await _categoryRepository.Upsert(model); // your repo inserts/updates based on Id
            return RedirectToAction(nameof(Index));
        }

        // ---------- DELETE ----------
        // (Consider also deleting the physical file after a successful delete if your repo returns the old path.)
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0) return BadRequest();

            // (Optional) fetch to delete file after successful delete
            var existing = await _categoryRepository.GetCategoryById(id);

            var result = await _categoryRepository.DeleteAsync(id);
            if (result)
            {
                if (!string.IsNullOrWhiteSpace(existing?.ImagePath))
                    DeletePhysicalFile(existing.ImagePath);
                return RedirectToAction(nameof(Index));
            }

            return StatusCode(500, "Failed to delete category.");
        }

        // ---------- FILE HELPERS ----------
        private static readonly string[] AllowedExt = { ".jpg", ".jpeg", ".png", ".webp" };
        private static readonly string[] AllowedMime = { "image/jpeg", "image/png", "image/webp" };
        private const long MaxBytes = 2 * 1024 * 1024; // 2MB



        private async Task<(bool ok, string? path, string? error)> SaveCategoryImageAsync(IFormFile file)
        {
            if (file.Length == 0) return (false, null, "Empty file.");
            if (file.Length > MaxBytes) return (false, null, "Image size must be less than 2MB.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExt.Contains(ext))
                return (false, null, "Only JPG, PNG, or WebP formats are allowed.");

            var mime = file.ContentType?.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(mime) || !AllowedMime.Contains(mime))
                return (false, null, "Unsupported image content type.");

            // ✅ Blob container setup
            var blobConn = _config["AzureStorage:ConnectionString"];  // from appsettings.json
            var containerName = _config["AzureStorage:ContainerName"]; // "uploads"

            // File name
            var fileName = $"cat-{Guid.NewGuid():N}{ext}";
            var blobPath = $"Categories/{fileName}";

            try
            {
                var blobServiceClient = new BlobServiceClient(blobConn);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var blobClient = containerClient.GetBlobClient(blobPath);
                await blobClient.UploadAsync(file.OpenReadStream(), overwrite: true);

                // ✅ Return public blob URL
                return (true, blobClient.Uri.ToString(), null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Upload failed: {ex.Message}");
            }
        }



        private void DeletePhysicalFile(string relativePath)
        {
            try
            {
                // Ignore if the path is invalid or not a blob URL
                if (string.IsNullOrWhiteSpace(relativePath))
                    return;

                // Extract filename if full URL is stored (optional)
                // Example: https://medhyaimages.blob.core.windows.net/uploads/Categories/cat-abc.png
                var uri = new Uri(relativePath);
                var blobPath = uri.AbsolutePath.TrimStart('/'); // uploads/Categories/...

                // Read from configuration
                var blobConn = _config["AzureStorage:ConnectionString"];
                var containerName = _config["AzureStorage:ContainerName"]; // "uploads"

                var blobServiceClient = new BlobServiceClient(blobConn);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                // Delete the blob if it exists
                var blobClient = containerClient.GetBlobClient(blobPath);
                blobClient.DeleteIfExists();
            }
            catch
            {
                // TODO: log the exception if needed
            }
        }


        // (You can remove this if unused)
        public IActionResult CreateBanner(Banner banner)
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
