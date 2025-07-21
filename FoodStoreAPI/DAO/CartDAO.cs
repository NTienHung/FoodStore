using FoodStoreAPI.DTO;
using FoodStoreAPI.Models;

namespace FoodStoreAPI.DAO
{
    public class CartDAO
    {
        public static void AddToCart(OrderDTO orderDTO)
        {
            try
            {
                //OrderItem orderItem = new OrderItem
                //{
                //    ProductId = orderDTO.
                //};
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<OrderItem> GetListCart()
        {
            List<OrderItem> listCart = new List<OrderItem>();
            try
            {
                listCart = FoodStoreContext.Ins.OrderItems.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listCart;
        }

        public static void OrderProduct(OrderDTO orderDTO, int accId)
        {
            Account account = FoodStoreContext.Ins.Accounts.FirstOrDefault(x => x.AccId == accId);
            if (account != null)
            {
                try
                {
                    Order order = new Order
                    {
                        CustomerId = account.AccId,
                        Gtotal = orderDTO.Gtotal,
                        Status = orderDTO.Status,
                        CreateAt = DateTime.Now,
                        OrderDate = DateTime.Now,
                    };
                    FoodStoreContext.Ins.Orders.Add(order);
                    FoodStoreContext.Ins.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public static void OrderItemsProduct(OrderItemDTO orderItemDTO, int orderId)
        {
            Order order = FoodStoreContext.Ins.Orders.FirstOrDefault(x => x.OrderId == orderId);
            if (order != null)
            {
                try
                {
                    OrderItem orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = orderItemDTO.ProductId,
                        Quantity = orderItemDTO.Quantity,
                        CreateAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                    };
                    FoodStoreContext.Ins.OrderItems.Add(orderItem);
                    FoodStoreContext.Ins.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public static void updateOrderById(OrderDTO orderdto, int orderId)
        {
            Order order = FoodStoreContext.Ins.Orders.FirstOrDefault(x => x.OrderId == orderId);
            if (order != null)
            {
                try
                {
                    order.UpdateAt = DateTime.Now;
                    order.Gtotal = orderdto.Gtotal;
                    FoodStoreContext.Ins.Orders.Update(order);
                    FoodStoreContext.Ins.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}

