using Medhya.API.Models;
using Medhya.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medhya.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            var result = await _orderRepository.AddAsync(order);
            return Ok(result);
        }

        [HttpPost("CreateTempOrder")]
        public async Task<IActionResult> CreateTempOrder([FromBody] TempOrder tempOrder)
       {
            var result = await _orderRepository.AddAsync(tempOrder);
            return Ok(result);
        }

        [HttpGet("GetOrders")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderRepository.GetAllAsync();
            if (orders == null || !orders.Any())
                return NotFound("No orders found.");
            return Ok(orders);
        }

        [HttpGet("GetOrderById/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return NotFound($"Order with ID {id} not found.");
            return Ok(order);
        }

        [HttpDelete("DeleteOrder/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderRepository.DeleteAsync(id);
            if (result == 0)
                return NotFound($"Order with ID {id} could not be deleted.");
            return Ok("Order deleted successfully.");
        }

        [HttpGet("GetOrderDetailsByUserId/{userId}")]
        public async Task<IActionResult> GetOrderDetailsByUserId(string userId)
        {
            var (orders, orderItems) = await _orderRepository.GetOrderDetailsByUserIdAsync(userId);
            if (orders == null || !orders.Any())
                return NotFound("No order details found for the user.");
            return Ok(new { Orders = orders, OrderItems = orderItems });
        }

        [HttpGet("GetTempOrderDetailsById/{tempOrderId}")]
        public async Task<IActionResult> GetTempOrderDetailsById(int tempOrderId)
        {
            var (tempOrder, tempOrderItems) = await _orderRepository.GetTempOrderDetailsByIdAsync(tempOrderId);
            if (tempOrder == null)
                return NotFound($"Temp order with ID {tempOrderId} not found.");
            return Ok(new { TempOrder = tempOrder, TempOrderItems = tempOrderItems });
        }
    }
}
