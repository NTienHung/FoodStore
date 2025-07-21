using Microsoft.AspNetCore.Mvc;
using FoodStoreAPI.DAO;
using FoodStoreAPI.DTO;

namespace FoodStoreAPI.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpPut("Customer/Account/Update/Information")]
        public IActionResult updateInformationAccountById(int id, AccountDTO accountDTO) 
        { 
            AccountDAO.updateInformationAccountById(id, accountDTO);
            return Ok();
        }
        [HttpGet("Customer/Account/Profile/{id}")]
        public IActionResult findAccountById(int id)
        {
            return Ok(AccountDAO.findAccountById(id));
        }
    }
}
