using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodStoreAPI.DAO;
using FoodStoreAPI.DTO;

namespace FoodStoreAPI.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        //GET:
        //[Authorize]
        [HttpGet("Customer/Orders/{cusId}")]
        public IActionResult GetOrder(int cusId)
        {
            return Ok(OrderDAO.getOrder(cusId));
        }
        //GET:
        //[Authorize]
        [HttpGet("Customer/OrderStatus/{cusId}")]
        public IActionResult getOrderByIdAndStatus(int cusId)
        {
            return Ok(OrderDAO.getOrderByIdAndStatus(cusId));
        }
        //GET:
        //[Authorize]
        [HttpGet("Customer/OrderItem")]
        public IActionResult GetOrderItem()
        {
            return Ok(OrderDAO.getOrderItem());
        }
        //GET:
        //Authorize]
        [HttpGet("Customer/Order/{id}")]
        public IActionResult GetOrderById(int id)
        {
            var order = OrderDAO.findOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }
       // [Authorize]
        [HttpGet("Customer/OrderItems/{id}")]
        public IActionResult GetOrderItemById(int id)
        {
            var order = OrderDAO.getOrderItemById(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }
        //PUT: 
        [HttpPut("Customer/Status/{id}")]
        public IActionResult updateOrderStatusById(int id, string status)
        {
            var order = OrderDAO.findOrderById(id);
            if(order == null)
            {
                return NotFound();
            }
            OrderDAO.updateOrderStatusById(id, status);
            return Ok();
        }
        //POST:
        [HttpPost("Customer/Add/Order")]
        public IActionResult AddNewOrder(OrderDTO orderDTO)
        {
            OrderDAO.AddNewOrder(orderDTO);
            return Ok();
        }
        //POST:
        [HttpPost("Customer/UpdateStatusGtotal/Order/{id}")]
        public IActionResult UpdateOrderStatusGtotalById(int id, [FromBody] float gtotal)
        {
            OrderDAO.updateOrderStatusGtotalById(id, gtotal);
            return Ok();
        }
        //POST:
        [HttpPost("Customer/Add/OrderItem")]
        public IActionResult AddNewOrderItem(OrderItemDTO orderItemDTO)
        {
            OrderDAO.AddNewOrderItem(orderItemDTO);
            return Ok();
        }
        //GET:
      //  [Authorize]
        [HttpGet("Customer/View/LastOrder")]
        public IActionResult GetLastOrder()
        {
            return Ok(OrderDAO.GetLastOrder());
        }
        //GET:
       // [Authorize]
        [HttpGet("Customer/View/OrderByGtotalCustomerStatus")]
        public IActionResult getOrderByGtotalCustomerStatus(float gtotalOrder, int accid)
        {
            return Ok(OrderDAO.getOrderByGtotalCustomerStatus(gtotalOrder, accid));
        }
    }
}
