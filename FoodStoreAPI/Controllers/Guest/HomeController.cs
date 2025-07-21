using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodStoreAPI.DAO;

namespace FoodStoreAPI.Controllers.Guest
{
    public class HomeController : ControllerBase
    {
        [HttpGet("Guest/Products/{cateId}")]
        public IActionResult GetProduct(int cateId)
        {
            return Ok(ProductDAO.getProduct(cateId));
        }
    }
}
