using Medhya.Admin.Models;

namespace Medhya.Admin.Repository

{
    public interface IcategoryRepository
    {
        public Task<Category> Upsert(Category category);
        public Task<IEnumerable<Category>> GetAllCategories();
        public Task<Category> GetCategoryById(int id);
    }
}
