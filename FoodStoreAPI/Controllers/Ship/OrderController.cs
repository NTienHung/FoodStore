using FoodStoreAPI.DAO;
using FoodStoreAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using FoodStoreAPI.DAO;

namespace FoodStoreAPI.Controllers.Ship
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        //GET:
        [HttpGet("Ship/OderForShip")]
        public ActionResult<IEnumerable<OrderLDTO>> GetOrderForShip()
        {
            return OrderDAO.getOrderForShip();
        }
        //GET:
        [HttpGet("Ship/OrderForShipFilter")]
        public ActionResult<IEnumerable<OrderLDTO>> GetOrderForShipFilter(string statusOrder)
        {
            return OrderDAO.getOrderForShipFilter(statusOrder);
        }
        //GET:
        [HttpGet("Ship/OrderForShipFilterAndSearch")]
        public ActionResult<IEnumerable<OrderLDTO>> GetOrderForShipFilterAndSearch(string searchOrder)
        {
            return OrderDAO.getOrderForShipFilterAndSearch(searchOrder);
        }
        //GET:
        [HttpGet("Ship/OrderItem")]
        public IActionResult getListOrderItem(int orderId)
        {
            return Ok(OrderDAO.getListOrderItem(orderId));
        }
        //GET:
        [HttpGet("Ship/OrderDetail")]
        public IActionResult getOrderDetail(int orderId)
        {
            return Ok(OrderDAO.getOrderDetail(orderId));
        }
        //PUT: 
        [HttpPut("Ship/Status/{id}")]
        public IActionResult getUpdateOrderById(int id, string status)
        {
            var order = OrderDAO.findOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            OrderDAO.updateOrderStatusById(id, status);
            return Ok();
        }
    }
}