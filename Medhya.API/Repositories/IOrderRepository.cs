using Medhya.API.Models;

namespace Medhya.API.Repository
{
    public interface IOrderRepository
    {

        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> GetByIdAsync(int id);
        Task<int> AddAsync(Order order);

        Task<int> DeleteAsync(int id);
        Task<TempOrder> GetTempOrderById(int id);



    }
}
