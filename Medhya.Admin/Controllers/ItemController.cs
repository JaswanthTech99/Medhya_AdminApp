using Medhya.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace Medhya.Admin.Controllers
{
    public class ItemController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Item item)
        {
            return View();
        }
    }
}
