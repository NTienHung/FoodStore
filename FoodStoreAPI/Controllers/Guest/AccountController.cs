using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using FoodStoreAPI.DAO;
using FoodStoreAPI.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FoodStoreAPI.Controllers.Guest
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("Guest/Account/Login")]
        public IActionResult Login(string username, string pass)
        {
            var user = AccountDAO.Login(username, pass);
            if (user != null && user.RoleId != 5)
            {
                var issuer = _configuration["jwt:issuer"];
                var audience = _configuration["jwt:audience"];
                var key = Encoding.ASCII.GetBytes(_configuration["jwt:key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                            new Claim("Id",Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Name, user.Username),
                            new Claim(JwtRegisteredClaimNames.Email, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            
                        }),
                    Expires = DateTime.Now.AddMinutes(30),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var stringToken = tokenHandler.WriteToken(token);
                return Ok(new { Token = stringToken, user });
            }
            return Unauthorized("Tai Khoan Chua ton tai. Ban co muon dang ki tai khoan khong");
        }

        [HttpPost("Guest/Account/Register")]
        public IActionResult register(AccountDTO accountDTO)
        {
            AccountDAO.Register(accountDTO);
            return Ok();
        }

        [HttpGet("Guest/Account/ExistAccount")]
        public IActionResult findAccountByUsernameAndEmail(string username, string email)
        {
            return Ok(AccountDAO.findAccountByUsernameAndEmail(username, email));
        }
        [HttpGet("Guest/Account/ForgotPassword")]
        public IActionResult findAccountByEmail(string email)
        {
            return Ok(AccountDAO.findAccountByEmail(email));
        }
        [HttpPut("Guest/Account/ChangePassword")]
        public IActionResult changePassword(string email, string pass)
        {
            AccountDAO.changePassword(email, pass);
            return Ok();
        }
    }
}
