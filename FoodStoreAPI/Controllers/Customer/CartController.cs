using Microsoft.AspNetCore.Mvc;
using FoodStoreAPI.DAO;
using FoodStoreAPI.DTO;
using FoodStoreAPI.Models;

namespace FoodStoreAPI.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        [HttpGet("Customer/Cart")]
        public ActionResult<IEnumerable<OrderItem>> GetListCart()
        {
            return CartDAO.GetListCart();
        }
        [HttpPost("Customer/AddToCart")]
        public IActionResult PostCart(OrderDTO orderDTO)
        {
            CartDAO.AddToCart(orderDTO);
            return Ok();
        }
        [HttpPost("Customer/OrderProduct")]
        public IActionResult PostOrderProduct(OrderDTO orderDTO, int accId)
        {
            CartDAO.OrderProduct(orderDTO, accId);
            return Ok();
        }

        [HttpPost("Customer/OrderItemsProduct")]
        public IActionResult PostOrderItemsProduct(OrderItemDTO orderItemDTO, int orderId)
        {
            CartDAO.OrderItemsProduct(orderItemDTO, orderId);
            return Ok();
        }

        [HttpPut("Customer/OrderUpdate/{id}")]
        public IActionResult PutOrderUpdate(OrderDTO orderDTO, int id)
        {
            CartDAO.updateOrderById(orderDTO, id);
            return Ok();
        }
    }
}
