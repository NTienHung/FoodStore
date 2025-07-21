using FoodStoreAPI.DAO;
using FoodStoreAPI.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodStoreAPI.Controllers.Sale
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpGet("Sale/Categories")]
        public IActionResult GetCate()
        {
            return Ok(ProductDAO.getCate());
        }
     
        [HttpPost("Sale/AddNewProduct")]
        public IActionResult AddNewProduct(ProductDTO productDTO)
        {
          var i =  ProductDAO.AddNewProduct(productDTO);
            if (i)
            {
                return Ok("add san pham thanh cong");
            }          
            return BadRequest("add loi roi");
        }
      
        [HttpPut("Sale/UpdateProduct/{proId}")]
        public IActionResult UpdateProduct(int proId, productLDTO productDTO)
        {
            ProductDAO.UpdateProduct(proId, productDTO);
            return Ok();
        }
        [HttpGet("Sale/ProductByLastList")]
        public IActionResult findProductByLastList(int accId)
        {
            return Ok(ProductDAO.findProductByLastList(accId));
        }
       
        [HttpPut("Sale/DeleteProduct")]
        public IActionResult deteleProduct(int proId)
        {
            ProductDAO.deteleProduct(proId);
            return Ok();
        }
    
        [HttpGet("Sale/Product")]
        public IActionResult findProductById(int proId)
        {
            return Ok(ProductDAO.findProductById(proId));
        }
    }
}
