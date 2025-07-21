using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodStoreAPI.DAO;
using FoodStoreAPI.DTO;

namespace FoodStoreAPI.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("Customer/Products/{cateId}")]
        public IActionResult GetProduct(int cateId)
        {
            return Ok(ProductDAO.getProduct(cateId));
        }
        [HttpPost("Customer/AddProduct")]
        public IActionResult PostProduct(ProductDTO product)
        {
            ProductDAO.saveProduct(product);
            return Ok();
        }
        [HttpDelete("Customer/Delete/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var p = ProductDAO.findProductById(id);
            if (p == null)
            {
                return NotFound();
            }
            ProductDAO.deteleProduct(p.ProId);
            return Ok();
        }
        [HttpPut("Customer/Update/{id}")]
        public IActionResult UpdateProduct(int id, ProductDTO productdto)
        {
            var product = ProductDAO.findProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            ProductDAO.updateProductById(product.ProId, productdto);
            return Ok();
        }
        [HttpGet("Customer/Details/{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = ProductDAO.findProductById(id);

            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
    }
}
