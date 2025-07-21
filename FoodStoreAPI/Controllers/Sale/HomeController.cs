using FoodStoreAPI.DAO;
using FoodStoreAPI.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodStoreAPI.Controllers.Sale
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {

        [HttpGet("Sale/Products/{cateId}")]
        public IActionResult GetProduct(int cateId)
        {
            return Ok(ProductDAO.GetProduct(cateId));
        }

        [HttpPost("Sale/AddProduct")]
        public IActionResult PostProduct(ProductDTO product)
        {
            ProductDAO.saveProduct(product);
            return Ok();
        }
 
        [HttpDelete("Sale/Delete/Product/{id}")]
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
   
        [HttpPut("Sale/Update/Product/{id}")]
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
   
        [HttpGet("Sale/Details/Product/{id}")]
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
