using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Medhya.Admin.Models;
using Medhya.Admin.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Medhya.Admin.Controllers
{
    public class BannerController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IBannerRepository _repository;
        private readonly IConfiguration _config;
        public BannerController(IWebHostEnvironment env, IBannerRepository repository, IConfiguration config)
        {
            _env = env;
            _repository = repository;
            var home = Environment.GetEnvironmentVariable("HOME") ?? env.ContentRootPath; // Azure: /home
            _config = config;
        }
        public async Task<IActionResult> Index()
        {
            var banners = await _repository.GetAllBannersAsync(false);
            return View(banners);
        }
        public IActionResult Create()
        {
            return View("Save", new Banner());
        }
        public async Task<IActionResult> Edit(int id)
        {
            var banner = await _repository.GetBannerByIdAsync(id);
            if (banner == null)
                return NotFound();

            return View("Save", banner); // use same Save view
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Save(Banner banner)
        {
            if (!ModelState.IsValid)
                return View(banner);

            // handle image upload
            if (banner.MediaFile != null && banner.MediaFile.Length > 0)
            {
                var conn = _config["AzureStorage:ConnectionString"];
                var containerName = _config["AzureStorage:ContainerName"];
                var blobServiceClient = new BlobServiceClient(conn);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var ext = Path.GetExtension(banner.MediaFile.FileName);
                var fileName = $"banners/{Guid.NewGuid():N}{ext}";
                var blobClient = containerClient.GetBlobClient(fileName);

                using (var stream = banner.MediaFile.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                banner.BannerUrl = blobClient.Uri.ToString();
            }

            await _repository.SaveBannerAsync(banner);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var banner = await _repository.GetBannerByIdAsync(id);
            if (banner != null && !string.IsNullOrEmpty(banner.BannerUrl))
            {
                try
                {
                    await DeleteBannerFromBlobAsync(banner.BannerUrl);
                }
                catch
                {
                    // TODO: log but ignore errors
                }
            }
            await _repository.DeleteBannerAsync(id, "admin");
            return RedirectToAction("Index");
        }

        private async Task DeleteBannerFromBlobAsync(string blobUrl)
        {
            var conn = _config["AzureStorage:ConnectionString"];
            var containerName = _config["AzureStorage:ContainerName"];

            var blobServiceClient = new BlobServiceClient(conn);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var uri = new Uri(blobUrl);
            var blobPath = uri.AbsolutePath.TrimStart('/'); // uploads/banners/xxx.png

            var blobClient = containerClient.GetBlobClient(blobPath);
            await blobClient.DeleteIfExistsAsync();
        }

    }
}
