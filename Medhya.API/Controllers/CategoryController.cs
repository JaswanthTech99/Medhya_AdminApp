using Medhya.API.Model;
using Medhya.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medhya.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository) 
        {
            _categoryRepository = categoryRepository;
        }

        [HttpPost("CreateCategory")]
        public async Task<IActionResult> CreateCategory([FromBody] Category category) 
        {
            var result = await _categoryRepository.Upsert(category);
            return Ok(result);
        }

        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryRepository.GetAllCategories();
            if(categories == null)
                return NotFound();
            return Ok(categories);
        }

        [HttpPost("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory([FromBody] Category category)
        {
            var categoryItem = await _categoryRepository.GetCategoryById(category.Id);
            if(categoryItem == null)
                return Ok("No record found to update.");
            var result = await _categoryRepository.Upsert(category);
            return Ok(result);
        }

        [HttpGet("GetCategoryById")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryRepository.GetCategoryById(id);
            if(category == null) NoContent();
            return Ok(category);
        }
    }
}
