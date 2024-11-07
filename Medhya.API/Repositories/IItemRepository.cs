using Medhya.API.Models;
namespace Medhya.API.Repository
{
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetAllAsync();
        Task<Item> GetByIdAsync(int id);
        Task<int> AddAsync(Item item);
       // Task<int> UpdateAsync(Item item);
        Task<int> DeleteAsync(int id);
    }
}
