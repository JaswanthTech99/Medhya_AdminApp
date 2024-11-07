using Azure;
using Medhya.API.Interfaces;
using Medhya.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Medhya.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private List<WebUser> lstUser = new List<WebUser>
        {
            new WebUser(){ Email="e1@e1.com"  , Password="p1",Username="n1", Role="user"},
            new WebUser(){ Email="e2@e2.com"  , Password="p2",Username="n2", Role="user"}
        };

        private readonly IConfiguration _configuration;
        private readonly ISMSService _smsService;

        //public AuthenticateController(
        //    UserManager<IdentityUser> userManager,
        //    RoleManager<IdentityRole> roleManager,
        //    IConfiguration configuration)
        //{
        //    _userManager = userManager;
        //    _roleManager = roleManager;
        //    _configuration = configuration;
        //}

        public AuthenticateController(
          IConfiguration configuration,ISMSService smsService)
        {
            _configuration = configuration;
            _smsService = smsService;
        }
        [HttpPost]
        [Route("login")]
        public WebUser Login([FromBody] WebUser model)
        {
            var user = lstUser.SingleOrDefault(u => u.Username == model.Username && u.Password == model.Password);// //_userManager.FindByNameAsync(model.Username);
            if (user == null)
                return null;

            var userRoles = user.Role;//await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, userRoles)
            };

            //foreach (var userRole in userRoles)
            //{
            //    authClaims.Add();
            //}

            var token = GetToken(authClaims);

            user.Token = new JwtSecurityTokenHandler().WriteToken(token);
            user.Password = null;
            return user;
            //return Ok(new
            //{
            //    token = new JwtSecurityTokenHandler().WriteToken(token),
            //    expiration = token.ValidTo
            //});
          
           // return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] WebUser model)
        {
            var userExists = lstUser.SingleOrDefault(u => u.Username == model.Username);//await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new WebUserResponse { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = "";//await _userManager.CreateAsync(user, model.Password);
            //if (!result.Succeeded)
            //    return StatusCode(StatusCodes.Status500InternalServerError, new WebUserResponse { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok(new WebUserResponse { Status = "Success", Message = "User created successfully!" });
        }


        [HttpPost]
        [Route("SendSMS")]
        public async Task<bool> SendSMSAsync(string mobileNumber, string username)
        {
            return await _smsService.SendSMSAsync(mobileNumber, username);
        }

        [HttpPost]
        [Route("VerfiySMS")]
        public async Task<bool> VerfiySMSAsync(int enteredCode, string username)
        {
            return await _smsService.VerfiySMSAsync(enteredCode, username);
        }
        //[HttpPost]
        //[Route("register-admin")]
        //public async Task<IActionResult> RegisterAdmin([FromBody] WebUser model)
        //{
        //    var userExists = lstUser.SingleOrDefault(u => u.Username == model.Username);// await _userManager.FindByNameAsync(model.Username);
        //    if (userExists != null)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new WebUserResponse { Status = "Error", Message = "User already exists!" });

        //    IdentityUser user = new()
        //    {
        //        Email = model.Email,
        //        SecurityStamp = Guid.NewGuid().ToString(),
        //        UserName = model.Username
        //    };
        //    var result = await _userManager.CreateAsync(user, model.Password);
        //    if (!result.Succeeded)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new WebUserResponse { Status = "Error", Message = "User creation failed! Please check user details and try again." });

        //    if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
        //        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        //    if (!await _roleManager.RoleExistsAsync(UserRoles.User))
        //        await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

        //    if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
        //    {
        //        await _userManager.AddToRoleAsync(user, UserRoles.Admin);
        //    }
        //    if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
        //    {
        //        await _userManager.AddToRoleAsync(user, UserRoles.User);
        //    }
        //    return Ok(new WebUserResponse { Status = "Success", Message = "User created successfully!" });
        //}

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}