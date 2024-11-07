using Medhya.API.Model;
using Medhya.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Medhya.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<SecurityController> _logger;
        public SecurityController(IUserRepository userRepository, ILogger<SecurityController> logger)
        {
            this._userRepository = userRepository;
            this._logger = logger;
        }

        [HttpPost]
        [Route("RegisterUser")]
        public IActionResult RegisterUser([FromBody] User user)
        {
            try
            {
                _logger.LogInformation("User registration process started");
                var response = _userRepository.CreateUser(user);
                return Ok(response);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
