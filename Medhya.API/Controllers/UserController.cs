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
            //var result = await _userRepository.CreateUser(user);
            //return Ok(result);

            try
            {
               
                var result = await _userRepository.CreateUser(user);

                return result.ErrorNo switch
                {
                    1 => Ok(new { Message = "User registered successfully.", UserId = result.Message }),
                    -99 => Conflict(new { Error = "Mobile number already exists." }),
                    _ => StatusCode(500, new { Error = "An error occurred during registration." })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Internal server error." });
            }
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
            //if (result)
            //    return NotFound("No user found to update.");
            //return Ok("User updated successfully.");
            return result.ErrorNo switch
            {
                1 => Ok(new { Message = "User Mobile Number Updated successfully."}),
                -99 => Conflict(new { Error = "Mobile number already exists." }),
                _ => StatusCode(500, new { Error = "An error occurred during registration." })
            };
        }
    }
}
