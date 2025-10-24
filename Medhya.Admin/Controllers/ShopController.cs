using Medhya.Admin.Models;
using Medhya.Admin.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Medhya.Admin.Controllers
{
    public class ShopController : Controller
    {
        private readonly IShopService _shopService;

        public ShopController(IShopService shopService)
        {
            _shopService = shopService;
        }
        public IActionResult Index()
        {
            var categories = _shopService.GetCategories(); // List<Category>
            return View(categories);
        }
        public IActionResult LoadItems(int categoryId)
        {
            var items = _shopService.GetItemsByCategory(categoryId); // List<ItemList> including UOMs
            return PartialView("_ItemListPartial", items);
        }
        [HttpPost]
        public IActionResult CreateOrder([FromBody] CartOrderItems cartOrder)
        {
            if (cartOrder == null || cartOrder.cartItems == null)
            {
                return BadRequest("Cart is empty.");
            }

            // Save cart and items to database here using your repository/service
            _shopService.AddUpdateCart(cartOrder);

            return Ok("Order placed successfully");
        }

    }
}
