using Medhya.API.Models;

namespace Medhya.API.Repository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync(); // Fetch all orders
        Task<Order> GetByIdAsync(int id); // Get order by ID
        Task<int> AddAsync(Order order); // Add a new order
        Task<int> DeleteAsync(int id); // Delete order by ID
        Task<TempOrder> GetTempOrderById(int id); // Get temp order by ID
        Task<int> AddAsync(TempOrder order); // Add a new temporary order
        Task<(List<Order>, List<OrderItems>)> GetOrderDetailsByUserIdAsync(string userId); // Get order details by user ID
        Task<(TempOrder, List<TempOrderItems>)> GetTempOrderDetailsByIdAsync(int tempOrderId); // Get temp order details by tempOrderId
    }
}
