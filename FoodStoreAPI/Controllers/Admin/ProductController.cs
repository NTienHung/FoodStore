using Microsoft.AspNetCore.Mvc;
using FoodStoreAPI.DTO;
using FoodStoreAPI.DAO;
using Microsoft.EntityFrameworkCore;

namespace FoodStoreAPI.Controllers.Admin
{
    [Route("api/admin/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        // GET: api/admin/product
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var products = ProductDAO.getAllProduct(); 
            return Ok(products);
        }

        // GET: api/admin/product/{id}
        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = ProductDAO.findProductById(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        // POST: api/admin/product
        [HttpPost]
        public IActionResult CreateProduct([FromBody] ProductDTO productDto)
        {
            try
            {
                var success = ProductDAO.saveProduct(productDto);
                if (!success)
                    return BadRequest(new { message = "Failed to save product" });

                return Ok(new { message = "Product created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error"});
            }
        }


        // PUT: api/admin/product/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] ProductDTO productDto)
        {
            ProductDAO.updateProductById(id, productDto);
            return Ok(new { message = "Product updated successfully" });
        }

        // DELETE: api/admin/product/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            ProductDAO.deteleProduct(id);
            return Ok(new { message = "Product deleted successfully" });
        }

        // GET: api/admin/product/search?keyword=abc
        [HttpGet("search")]
        public IActionResult SearchProducts(string keyword)
        {
            var products = ProductDAO.SearchProducts(keyword);
            return Ok(products);
        }

        // GET: api/admin/product/filter?categoryId=1
        [HttpGet("filter")]
        public IActionResult FilterByCategory(int categoryId)
        {
            var products = ProductDAO.getProductByCate(categoryId);
            return Ok(products);
        }

        // GET: api/admin/product/orderby?orderby=ProductName&asc=true
        [HttpGet("orderby")]
        public IActionResult OrderProducts(string orderby, bool asc = true)
        {
            var products = ProductDAO.OrderProducts(orderby, asc);
            return Ok(products);
        }


        [HttpGet("category")]
        public IActionResult GetCategories()
        {
            var categories = CategoryDAO.getCategory();
            return Ok(categories);
        }

    }
}
