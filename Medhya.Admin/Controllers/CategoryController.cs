using Medhya.Admin.Models;
using Medhya.Admin.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medhya.Admin.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly IcategoryRepository _categoryRepository;
        public CategoryController(IcategoryRepository  categoryRepository)
        {
             _categoryRepository = categoryRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)

        {
            var result = _categoryRepository.Upsert(category);
            return View(category);
        } 
    }
}
