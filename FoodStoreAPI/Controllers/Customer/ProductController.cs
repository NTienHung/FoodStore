using Microsoft.AspNetCore.Mvc;
using FoodStoreAPI.DAO;
using FoodStoreAPI.DTO;

namespace FoodStoreAPI.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        [HttpGet("Customer/Product/Detail/{id}")]
        public IActionResult getProductById(int id)
        {
            var product = ProductDAO.findProductById(id);
            return Ok(product);
        }
        [HttpGet("Customer/Product/listProOrder/{id}")]
        public IActionResult getProductOrder(int id)
        {
            List<ProductDTO> product = ProductDAO.getProductOrder(id);
            return Ok(product);
        }
        //[HttpGet("Customer/Product/Category")]
        //public IActionResult getCategoryDTO(int id)
        //{
        //    List<CategoryDTO> cate = CategoryDAO.getCategoryDTO();
        //    return Ok(cate);
        //}
        [HttpGet("Customer/Product/finById")]
        public IActionResult findProductById(int id)
        {
            return Ok(ProductDAO.findProductById(id));
        }
        [HttpGet("Customer/Product/Category")]
        public IActionResult getCategoryDTO()
        {
            return Ok(CategoryDAO.getCategoryDTO());
        }
        [HttpGet("Customer/Product/On")]
        public IActionResult getProductOn()
        {
            return Ok(ProductDAO.getProductOn());
        }
    }
}
