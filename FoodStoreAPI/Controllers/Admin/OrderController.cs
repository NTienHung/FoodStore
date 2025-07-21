using FoodStoreAPI.DAO;
using FoodStoreAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FoodStoreAPI.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        //GET:
        [HttpGet("Admin/RevenueCaculate")]
        public IActionResult getRevenue(DateTime dateNow, DateTime dateSevenDay)
        {
            return Ok(RevenueDAO.getRevenueAdmin(dateNow, dateSevenDay));
        }
        //GET:
        [HttpGet("Admin/OrderCloseCaculate")]
        public IActionResult getListOrderRevenue(DateTime dateNow, DateTime dateSevenDay)
        {
            return Ok(OrderDAO.getListOrderRevenueAdmin(dateNow, dateSevenDay));
        }
        // GET:
        [HttpGet("Admin/GetAll")]
        public IActionResult GetAllOrders()
        {
            var orders = OrderDAO.GetAllOrderAdmin();
            return Ok(orders);
        }
        // POST: Thêm đơn hàng
        [HttpPost("Admin/Add")]
        public IActionResult AddOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest("ModelState Error: " + string.Join(" | ", errorList));
            }

            order.CreateAt = DateTime.Now;
            order.UpdateAt = DateTime.Now;

            var result = OrderDAO.Add(order);
            return result ? Ok("Added") : BadRequest("Failed");
        }

        // PUT: Sửa đơn hàng
        [HttpPut("Admin/Update")]
        public IActionResult UpdateOrder([FromBody] Order order)
        {
            var result = OrderDAO.Update(order);
            return result ? Ok("Updated") : BadRequest("Failed");
        }

        // DELETE: Xoá đơn hàng
        [HttpDelete("Admin/Delete/{id}")]
        public IActionResult DeleteOrder(int id)
        {
            var result = OrderDAO.Delete(id);
            return result ? Ok("Deleted") : BadRequest("Failed");   
        }

        [HttpGet("Admin/SearchByCustomerName")]
        public IActionResult SearchByCustomerName(string name)
        {
            var orders = OrderDAO.SearchByCustomerName(name);
            return Ok(orders);
        }


        [HttpGet("Admin/GetOrderDetails/{orderId}")]
        public IActionResult GetOrderDetails(int orderId)
        {
            var details = OrderDAO.getListOrderItem(orderId);
            return Ok(details);
        }

        [HttpGet("Admin/FilterByStatus")]
        public IActionResult FilterByStatus(string status)
        {
            var result = OrderDAO.GetOrdersByStatus(status);
            return Ok(result);
        }



    }
}
