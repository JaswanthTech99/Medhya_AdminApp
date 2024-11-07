using Medhya.API.Model;

namespace Medhya.API.Repositories;

public interface ICategoryRepository
{
    public Task<Category> Upsert(Category category);
    public Task<IEnumerable<Category>> GetAllCategories();
    public Task<Category> GetCategoryById(int id);

}

