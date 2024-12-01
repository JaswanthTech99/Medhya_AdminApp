using Medhya.API.Model;
using Medhya.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Medhya.API.Model.User;

namespace Medhya.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            var result = await _userRepository.CreateUser(user);
            return Ok(result);
        }

        [HttpPost("InsertUserAddress")]
        public async Task<IActionResult> InsertUserAddress(int userId, [FromBody] IEnumerable<UserAddress> addressDetails)
        {
            var result = await _userRepository.InsertUserAddress(userId, addressDetails);
            return Ok(result);
        }

        [HttpPost("UpdateUserAddress")]
        public async Task<IActionResult> UpdateUserAddress(int addressId, int userId, [FromBody] IEnumerable<UserAddress> addressDetails)
        {
            var result = await _userRepository.UpdateUserAddress(addressId, userId, addressDetails);
            return Ok(result);
        }

        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            var result = await _userRepository.UpdateUser(user);
            if (result == 0)
                return NotFound("No user found to update.");
            return Ok("User updated successfully.");
        }
    }
}
