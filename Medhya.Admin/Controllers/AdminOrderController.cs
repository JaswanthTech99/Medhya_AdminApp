using Medhya.Admin.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace Medhya.Admin.Controllers
{
    public class AdminOrderController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IItemRepository _itemRepository;


        public AdminOrderController(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
            // Create HttpClient instance for testing
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7047/api/") // Replace with your API base URL
            };
        }
        // GET: Call API and retrieve data
        public async Task<IActionResult> Index()
        {
            // Call GET API endpoint
            var response = await _httpClient.GetAsync("values"); // Replace "values" with your API endpoint
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var values = JsonSerializer.Deserialize<string[]>(data); // Assuming the API returns a string array
                ViewBag.Data = values;
            }
            else
            {
                ViewBag.Data = "Error fetching data from API.";
            }

            return View();
        }
        public IActionResult PostData()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PostData(TempOrder order)
        {
            var jsonContent = new StringContent(
            JsonSerializer.Serialize(order),
            Encoding.UTF8,
            "application/json");

            // Call POST API endpoint
            var response = await _httpClient.PostAsync("Order/CreateOrder", jsonContent); // Replace "values" with your API endpoint
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Content($"API Response: {result}");
            }

            return Content("Error posting data to API.");
        }
        [HttpPost]
        public async Task<IActionResult> CreateCart(TempOrder tempOrder)
        {
            return View();
        }
        public async Task<IActionResult> Itemlist()

        {
            try
            {
                var data = await _itemRepository.GetAllCategorieswithItems();
                return View(data);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return View("Error", ex.Message);
            }
        }
        [HttpPost]
        public IActionResult AddToCart(int itemId, string itemName, int quantity, decimal price)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();

            var existingItem = cart.FirstOrDefault(c => c.ItemId == itemId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ItemId = itemId,
                    ItemName = itemName,
                    Quantity = quantity,
                    Price = price
                });
            }

            HttpContext.Session.Set("Cart", cart);

            return Json(new { success = true, message = "Item added to cart." });
        }

    }

    public class TempOrder
    {
        public int Id { get; set; }
        public int userId { get; set; }
        public int ItemCount { get; set; }
        public decimal OrderAmount { get; set; }
        public string? TransactionType { get; set; }
        public string? PaymentType { get; set; }
        public decimal DeliveryCharges { get; set; }

        public string? OrderStatus { get; set; }
        public DateTime? OrderDate { get; set; }
        // public DateTime? OrderTime { get; set; }
        public List<TempOrderItems>? Items { get; set; }

    }
    public class CartItem
    {
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
    public class TempOrderItems
    {
        public int Id { get; set; }
        public int FK_TempOrderId { get; set; }
        public int FK_ItemId { get; set; }
        public string? FK_UOM { get; set; }
        public string? ItemStatus { get; set; }
        public decimal ItemPrice { get; set; }
        public int ItemQty { get; set; }
        public decimal ItemTotalAmount { get; set; }
        public decimal DiscountPrice { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
    }

}

