using FoodStoreAPI.DAO;
using FoodStoreAPI.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodStoreAPI.Controllers.Sale
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        //GET:
        [HttpGet("Sale/OderForSale")]
        public ActionResult<IEnumerable<OrderLDTO>> GetOrderForSale()
        {
            return OrderDAO.getOrderForSale();
        }
        //GET:
        [HttpGet("Sale/OrderForSaleFilter")]
        public ActionResult<IEnumerable<OrderLDTO>> GetOrderForSaleFilter(string statusOrder)
        {
            return OrderDAO.getOrderForSaleFilter(statusOrder);
        }
        //GET:
        [HttpGet("Sale/OrderForSaleFilterAndSearch")]
        public ActionResult<IEnumerable<OrderLDTO>> GetOrderForSaleFilterAndSearch(string searchOrder)
        {
            return OrderDAO.getOrderForSaleFilterAndSearch(searchOrder);
        }
        //GET:
        [HttpGet("Sale/OrderItem")]
        public IActionResult getListOrderItem(int orderId)
        {
            return Ok(OrderDAO.getListOrderItem(orderId));
        }
        //GET:
        [HttpGet("Sale/OrderDetail")]
        public IActionResult getOrderDetail(int orderId)
        {
            return Ok(OrderDAO.getOrderDetail(orderId));
        }
        //PUT: 
        [HttpPut("Sale/Status/{id}")]
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
        //GET:
        [HttpGet("Sale/OrderClose")]
        public IActionResult getListOrderClose(int accId)
        {
            return Ok(OrderDAO.getListOrderClose(accId));
        }
        //GET:
        [HttpGet("Sale/Revenue")]
        public IActionResult getRevenue(int accId)
        {
            return Ok(RevenueDAO.findRevenue(accId));
        }
        //GET:
        [HttpGet("Sale/RevenueCaculate")]
        public IActionResult getRevenue(int accId, DateTime dateNow, DateTime dateSevenDay)
        {
            return Ok(RevenueDAO.getRevenue(accId, dateNow, dateSevenDay));
        }
        //GET:
        [HttpGet("Sale/OrderCloseCaculate")]
        public IActionResult getListOrderRevenue(int accId, DateTime dateNow, DateTime dateSevenDay)
        {
            return Ok(OrderDAO.getListOrderRevenue(accId, dateNow, dateSevenDay));
        }
    }
}
