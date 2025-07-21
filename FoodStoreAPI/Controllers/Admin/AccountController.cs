using Microsoft.AspNetCore.Mvc;
using FoodStoreAPI.DAO;
using FoodStoreAPI.DTO;
using System.Text.RegularExpressions;
using System.Numerics;
using FoodStoreAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace FoodStoreAPI.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        [HttpGet("Admin/Accounts")]
        public ActionResult<IEnumerable<AccountLDTO>> GetAccount()
        {
            return AccountDAO.getAccount();
        }

        [HttpGet("Admin/Account/Search")]
        public ActionResult<IEnumerable<AccountLDTO>> GetAccountSearch(string? searchAccount, string? sort)
        {
            return AccountDAO.getAccountSearch(searchAccount, sort);
        }

        [HttpPut("Admin/Update/Account")]
        public IActionResult UpdateRoleAccount(int accId, int roleId)
        {
            AccountDAO.UpdateRoleAccount(accId, roleId);
            return Ok();
        }

        [HttpPost("Admin/AddNewUser/Account")]
        public ActionResult AddNewUser([FromBody] AccountDTO accountdto)
        {
            var isEmailExisted = FoodStoreContext.Ins.Accounts.Any(x => x.Email.Equals(accountdto.Email));
            var isUserExisted = FoodStoreContext.Ins.Accounts.Any(x => x.Username.Equals(accountdto.Username));
            if (isEmailExisted || isUserExisted)
            {
                return BadRequest("Email or username is existed!");
            }
            else
            {
                if (string.IsNullOrEmpty(accountdto.Name))
                {
                    return BadRequest("Your name must be valid!");
                }
                else
                {
                    if (!Regex.IsMatch(accountdto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        return BadRequest("Invalid email!");
                    }
                    else
                    {
                        if (accountdto.Phone.Length != 10 && !Regex.IsMatch(accountdto.Phone, @"^(03|05|07|08|09)\d{8}$"))
                        {
                            return BadRequest("Phone number must be 10 digits, and have a matching prefix!");
                        }
                        else
                        {
                            bool isValidPassword = accountdto.Password.Length > 8 && Regex.IsMatch(accountdto.Password, @"^[A-Z]") && Regex.IsMatch(accountdto.Password, @"\d") && Regex.IsMatch(accountdto.Password, @"[!@#$%^&*(),.?:{}|<>]");
                            if (string.IsNullOrEmpty(accountdto.Password) || !isValidPassword)
                            {
                                return BadRequest("Password must be at least 8 digits, must have an uppercase letter at the beginning of the string, must have at least one number, and must have at least one special character!");
                            }
                            else
                            {
                                Account account = new Account
                                {
                                    Email = accountdto.Email,
                                    Phone = accountdto.Phone,
                                    Name = accountdto.Name,
                                    Username = accountdto.Username,
                                    Password = AccountDAO.HashPassword(accountdto.Password),
                                    CreateAt = DateTime.Now,
                                    RoleId = accountdto.RoleId,
                                    Address = accountdto.Address
                                };
                                var context = FoodStoreContext.Ins;
                                context.Accounts.Add(account);
                                context.SaveChanges();
                            }
                        }
                    }
                }
            }            
            return Ok();
        }

        [HttpGet("Admin/AccountDetail/Account/{id}")]
        public ActionResult findAccount(int id)
        {
            return Ok(AccountDAO.findAccountById(id));
        }
    }
}
