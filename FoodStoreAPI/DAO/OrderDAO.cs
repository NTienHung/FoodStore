using FoodStoreAPI.DTO;
using FoodStoreAPI.DTOs;
using FoodStoreAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace FoodStoreAPI.DAO
{
    public class OrderDAO
    {
        //get last Order
        public static Order GetLastOrder()
        {
            FoodStoreContext context = new FoodStoreContext();
            Order order = new Order();
            try
            {
                order = context.Orders
                    .OrderByDescending(o => o.OrderId)
                    .FirstOrDefault();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Lỗi Database: {ex.InnerException?.Message}");
            }
            catch (Exception e)
            {
                throw new Exception($"Lỗi hệ thống: {e.Message}");
            }
            return order;
        }
        //create new order 
        public static void AddNewOrder(OrderDTO orderDTO)
        {
            FoodStoreContext context = new FoodStoreContext();
            try
            {
                Order order = new Order
                {
                    CustomerId = orderDTO.CustomerId,
                    Gtotal = orderDTO.Gtotal,
                    CreateAt = DateTime.Now,
                    Status = "Pending",
                    OrderDate = DateTime.Now
                };
                context.Orders.Add(order);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Lỗi Database: {ex.InnerException?.Message}");
            }
            catch (Exception e)
            {
                throw new Exception($"Lỗi hệ thống: {e.Message}");
            }
        }
        //create new itemOrder
        public static void AddNewOrderItem(OrderItemDTO orderItemDTO)
        {
            FoodStoreContext context = new FoodStoreContext();
            try
            {
                OrderItem orderItem = new OrderItem
                {
                    OrderId = orderItemDTO.OrderId,
                    ProductId = orderItemDTO.ProductId,
                    Quantity = orderItemDTO.Quantity,
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now
                };
                context.OrderItems.Add(orderItem);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Lỗi Database: {ex.InnerException?.Message}");
            }
            catch (Exception e)
            {
                throw new Exception($"Lỗi hệ thống: {e.Message}");
            }
        }
        //get list Order by customer's Id
        public static List<Order> getOrder(int cusId)
        {
            FoodStoreContext context = new FoodStoreContext();
            var listOrder = new List<Order>();
            try
            {
                listOrder = context.Orders
                    .Include(x => x.OrderItems)
                    .Where(x => x.CustomerId == cusId)
                    .Select(x => new Order
                    {
                        OrderId = x.OrderId,
                        CustomerId = x.CustomerId,
                        Gtotal = x.Gtotal,
                        CreateAt = x.CreateAt,
                        UpdateAt = x.UpdateAt,
                        Status = x.Status
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listOrder;
        }
        //get list Order by customer's Id and status "Pending"
        public static List<Order> getOrderByIdAndStatus(int cusId)
        {
            FoodStoreContext context = new FoodStoreContext();
            var listOrder = new List<Order>();
            try
            {
                listOrder = context.Orders
                    .Include(x => x.OrderItems)
                    .Where(x => x.CustomerId == cusId && x.Status == "Pending")
                    .Select(x => new Order
                    {
                        OrderId = x.OrderId,
                        CustomerId = x.CustomerId,
                        Gtotal = x.Gtotal,
                        CreateAt = x.CreateAt,
                        UpdateAt = x.UpdateAt,
                        Status = x.Status
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listOrder;
        }
        //get list Order
        public static List<OrderLDTO> getOrderForSale()
        {
            FoodStoreContext context = new FoodStoreContext();
            var listOrder = new List<OrderLDTO>();
            try
            {
                listOrder = context.Orders.Include(x => x.Customer)
                            .Select(x => new OrderLDTO
                            {
                                OrderId = x.OrderId,
                                CustomerName = x.Customer.Name,
                                Gtotal = x.Gtotal,
                                OrderDate = x.OrderDate,
                                Status = x.Status
                            })
                            .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listOrder;
        }
        //get list Order
        public static List<OrderLDTO> getOrderForSaleFilter(string statusOrder)
        {
            FoodStoreContext context = new FoodStoreContext();
            var listOrder = new List<OrderLDTO>();
            try
            {
                listOrder = context.Orders.Include(x => x.Customer)
                            .Where(x => (x.Status.Equals(statusOrder)))
                            .Select(x => new OrderLDTO
                            {
                                OrderId = x.OrderId,
                                CustomerName = x.Customer.Name,
                                Gtotal = x.Gtotal,
                                OrderDate = x.OrderDate,
                                Status = x.Status
                            })
                            .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listOrder;
        }
        //get list Order
        public static List<OrderLDTO> getOrderForSaleFilterAndSearch(string searchOrder)
        {
            FoodStoreContext context = new FoodStoreContext();
            var listOrder = new List<OrderLDTO>();
            try
            {
                listOrder = context.Orders.Include(x => x.Customer)
                            .Where(x => (x.Customer.Name.ToLower().Contains(searchOrder.ToLower())
                        || x.Gtotal.ToString().Contains(searchOrder)
                        || x.Status.ToLower().Contains(searchOrder.ToLower())
                        || x.OrderId.ToString().Contains(searchOrder)
                        || x.OrderDate.ToString().Contains(searchOrder))
                        && (x.Status.Equals("Pending")
                                      || x.Status.Equals("Accept Order")
                                      || x.Status.Equals("Delivering")
                                      || x.Status.Equals("Successful Delivery")
                                      || x.Status.Equals("Delivery Failed")
                                      || x.Status.Equals("Received")))
                            .Select(x => new OrderLDTO
                            {
                                OrderId = x.OrderId,
                                CustomerName = x.Customer.Name,
                                Gtotal = x.Gtotal,
                                OrderDate = x.OrderDate,
                                Status = x.Status
                            })
                            .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listOrder;
        }
        //get list OrderItem
        public static List<OrderItemDTO> getOrderItem()
        {
            FoodStoreContext context = new FoodStoreContext();
            List<OrderItemDTO> list = new List<OrderItemDTO>();
            try
            {
                list = context.OrderItems
                    .Include(x => x.Product)
                    .Select(o => new OrderItemDTO
                    {
                        Images = o.Product.Images,
                        Name = o.Product.Name,
                        Price = o.Product.Price,
                        OrderId = o.OrderId,
                        ProductId = o.ProductId
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return list;
        }

        public static List<OrderItemDTO> getListOrderItem(int orderId)
        {
            FoodStoreContext context = new FoodStoreContext();
            List<OrderItemDTO> list = new List<OrderItemDTO>();
            try
            {
                list = context.OrderItems
                    .Include(x => x.Product)
                    .Where(x => x.OrderId == orderId)
                    .Select(o => new OrderItemDTO
                    {
                        OrderItemId = o.OrderItemId,
                        Images = o.Product.Images,
                        Name = o.Product.Name,
                        Price = o.Product.Price,
                        OrderId = o.OrderId,
                        ProductId = o.ProductId,
                        Quantity = o.Quantity,
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return list;
        }

        public static OrderLDTO getOrderDetail(int orderId)
        {
            FoodStoreContext context = new FoodStoreContext();
            OrderLDTO order = new OrderLDTO();
            try
            {
                order = context.Orders
                    .Include(x => x.Customer)
                    .Where(x => x.OrderId == orderId)
                    .Select(c => new OrderLDTO
                    {
                        OrderId = c.OrderId,
                        CustomerId = c.CustomerId,
                        Gtotal = c.Gtotal,
                        CreateAt = c.CreateAt,
                        UpdateAt = c.UpdateAt,
                        Status = c.Status,
                        OrderDate = c.OrderDate,
                        CustomerName = c.Customer.Name,
                        CustomerPhone = c.Customer.Phone,
                        CustomerAddress = c.Customer.Address

                    })
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return order;
        }

        public static List<OrderItemDTO> getOrderItemById(int orderId)
        {
            FoodStoreContext context = new FoodStoreContext();
            List<OrderItemDTO> list = new List<OrderItemDTO>();
            try
            {
                list = context.OrderItems
                    .Include(x => x.Product)
                    .Where(x => x.OrderId == orderId)
                    .Select(o => new OrderItemDTO
                    {
                        Images = o.Product.Images,
                        Name = o.Product.Name,
                        Price = o.Product.Price,
                        OrderId = o.OrderId,
                        ProductId = o.ProductId,
                        OrderItemId = o.OrderItemId,
                        Quantity = o.Quantity
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return list;
        }

        //update Order status
        public static void updateOrderStatusById(int orderId, string status)
        {
            Order order = FoodStoreContext.Ins.Orders.FirstOrDefault(x => x.OrderId == orderId);
            if (order != null)
            {
                try
                {
                    order.UpdateAt = DateTime.Now;
                    order.Status = status;
                    FoodStoreContext.Ins.Orders.Update(order);
                    FoodStoreContext.Ins.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
        //update Order status and gtotal
        public static void updateOrderStatusGtotalById(int orderId, float gtotal)
        {
            Order order = FoodStoreContext.Ins.Orders.FirstOrDefault(x => x.OrderId == orderId);
            if (order != null)
            {
                try
                {
                    order.UpdateAt = DateTime.Now;
                    order.Status = "Accept Order";
                    order.Gtotal = gtotal;
                    FoodStoreContext.Ins.Orders.Update(order);
                    FoodStoreContext.Ins.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
        //get Order by Id
        public static Order findOrderById(int orderId)
        {
            FoodStoreContext context = new FoodStoreContext();
            Order order = new Order();
            try
            {
                order = context.Orders
                    .Where(x => x.OrderId == orderId)
                    .Select(x => new Order
                    {
                        OrderId = x.OrderId,
                        CustomerId = x.CustomerId,
                        Gtotal = x.Gtotal,
                        CreateAt = x.CreateAt,
                        UpdateAt = x.UpdateAt,
                        Status = x.Status
                    }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return order;
        }
        public static List<OrderItemDTO> getListOrderClose(int accId)
        {
            FoodStoreContext context = new FoodStoreContext();
            List<OrderItemDTO> list = new List<OrderItemDTO>();
            try
            {
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);

                list = context.OrderItems
                        .Include(x => x.Order)
                        .Include(x => x.Product)
                        .Where(x => x.Order.UpdateAt >= today && x.Order.UpdateAt < tomorrow
                                    && x.Order.Status == "Close Order"
                                    && x.Product.AccId == accId)
                        .Select(x => new OrderItemDTO
                        {
                            Price = x.Product.Price,
                            OriginalPrice = x.Product.OriginalPrice
                        })
                        .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return list;
        }
        public static List<OrderItemDTO> getListOrderRevenue(int accId, DateTime dateNow, DateTime dateSevenDay)
        {
            FoodStoreContext context = new FoodStoreContext();
            List<OrderItemDTO> list = new List<OrderItemDTO>();
            try
            {
                list = context.OrderItems
                        .Include(x => x.Order)
                        .Include(x => x.Product)
                        .Where(x => x.Order.UpdateAt >= dateSevenDay && x.Order.UpdateAt <= dateNow && x.Order.Status == "Close Order" && x.Product.AccId == accId)
                        .Select(o => new OrderItemDTO
                        {
                            Images = o.Product.Images,
                            Name = o.Product.Name,
                            Price = o.Product.Price,
                            OrderId = o.OrderId,
                            ProductId = o.ProductId,
                            OrderItemId = o.OrderItemId,
                            Quantity = o.Quantity,
                            CreateAt = o.Order.OrderDate,
                            OriginalPrice = o.Product.OriginalPrice
                        })
                        .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return list;
        }
      
        public static List<OrderItemDTO> getListOrderRevenueAdmin(DateTime dateNow, DateTime dateSevenDay)
        {
            FoodStoreContext context = new FoodStoreContext();
            List<OrderItemDTO> list = new List<OrderItemDTO>();
            try
            {
                list = context.OrderItems
                        .Include(x => x.Order)
                        .Include(x => x.Product)
                        .Where(x => x.Order.UpdateAt >= dateSevenDay && x.Order.UpdateAt <= dateNow && x.Order.Status == "Close Order")
                        .Select(o => new OrderItemDTO
                        {
                            Images = o.Product.Images,
                            Name = o.Product.Name,
                            Price = o.Product.Price,
                            OrderId = o.OrderId,
                            ProductId = o.ProductId,
                            OrderItemId = o.OrderItemId,
                            Quantity = o.Quantity,
                            CreateAt = o.Order.OrderDate,
                            OriginalPrice = o.Product.OriginalPrice
                        })
                        .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return list;
        }
        
        //get list Order
        public static List<OrderLDTO> getOrderForShip()
        {
            FoodStoreContext context = new FoodStoreContext();
            var listOrder = new List<OrderLDTO>();
            try
            {
                listOrder = context.Orders.Include(x => x.Customer)
                            .Where(x => x.Status.Equals("Pending") || x.Status.Equals("Delivering") || x.Status.Equals("Successful Delivery"))
                            .Select(x => new OrderLDTO
                            {
                                OrderId = x.OrderId,
                                CustomerName = x.Customer.Name,
                                Gtotal = x.Gtotal,
                                OrderDate = x.OrderDate,
                                Status = x.Status
                            })
                            .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listOrder;
        }
        //get list Order
        public static List<OrderLDTO> getOrderForShipFilter(string statusOrder)
        {
            FoodStoreContext context = new FoodStoreContext();
            var listOrder = new List<OrderLDTO>();
            try
            {
                listOrder = context.Orders.Include(x => x.Customer)
                            .Where(x => (x.Status.Equals(statusOrder)))
                            .Select(x => new OrderLDTO
                            {
                                OrderId = x.OrderId,
                                CustomerName = x.Customer.Name,
                                Gtotal = x.Gtotal,
                                OrderDate = x.OrderDate,
                                Status = x.Status
                            })
                            .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listOrder;
        }
        //get list Order
        public static List<OrderLDTO> getOrderForShipFilterAndSearch(string searchOrder)
        {
            FoodStoreContext context = new FoodStoreContext();
            var listOrder = new List<OrderLDTO>();
            try
            {
                listOrder = context.Orders.Include(x => x.Customer)
                            .Where(x => x.Status.Equals("Pending") || x.Status.Equals("Delivering") || x.Status.Equals("Successful Delivery"))
                        .Where(x => x.Customer.Name.ToLower().Contains(searchOrder.ToLower())
                        || x.Gtotal.ToString().Contains(searchOrder)
                        || x.Status.ToLower().Contains(searchOrder.ToLower())
                        || x.OrderId.ToString().Contains(searchOrder)
                        || x.OrderDate.ToString().Contains(searchOrder))
                            .Select(x => new OrderLDTO
                            {
                                OrderId = x.OrderId,
                                CustomerName = x.Customer.Name,
                                Gtotal = x.Gtotal,
                                OrderDate = x.OrderDate,
                                Status = x.Status
                            })
                            .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listOrder;
        }
        public static OrderLDTO getOrderByGtotalCustomerStatus(float gtotalOrder, int accid)
        {
            FoodStoreContext context = new FoodStoreContext();
            var Order = new OrderLDTO();
            try
            {
                Order = context.Orders.Where(x => x.Gtotal == gtotalOrder && x.CustomerId == accid && x.Status == "Pending")
                        .OrderByDescending(x => x.OrderDate)
                        .Select(x => new OrderLDTO
                        {
                            OrderId = x.OrderId,
                        })
                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Order;
        }

        public static List<Order> GetAllOrders()
        {
            using (var context = new FoodStoreContext())
            {
                return context.Orders.ToList();
            }
        }

        public static List<OrderLDTO> GetAllOrderAdmin()
        {
            using (var context = new FoodStoreContext())
            {
                return context.Orders
                    .Include(x => x.Customer)
                    .Select(x => new OrderLDTO
                    {
                        OrderId = x.OrderId,
                        CustomerId = x.CustomerId,
                        CustomerName = x.Customer != null ? x.Customer.Name : "", 
                        Gtotal = x.Gtotal,
                        Status = x.Status,
                        OrderDate = x.OrderDate,
                        CreateAt = x.CreateAt
                    })
                    .ToList();
            }
        }

        public static bool Add(Order order)
        {
            using var db = new FoodStoreContext();
            db.Orders.Add(order);
            return db.SaveChanges() > 0;
        }

        public static bool Update(Order order)
        {
            using var db = new FoodStoreContext();
            var existing = db.Orders.FirstOrDefault(o => o.OrderId == order.OrderId);
            if (existing == null) return false;

            existing.CustomerId = order.CustomerId;
            existing.Gtotal = order.Gtotal;
            existing.OrderDate = order.OrderDate;
            existing.Status = order.Status;
            existing.UpdateAt = DateTime.Now;

            return db.SaveChanges() > 0;
        }

        public static bool Delete(int id)
        {
            using var db = new FoodStoreContext();
            var order = db.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null) return false;

            order.Status = "Deleted";
            order.UpdateAt = DateTime.Now;
            db.Orders.Update(order);

            return db.SaveChanges() > 0;
        }

        public static List<OrderLDTO> SearchByCustomerName(string customerName)
        {
            using var context = new FoodStoreContext();
            return context.Orders
                .Include(x => x.Customer)
                .Where(x => x.Customer.Name.Contains(customerName))
                .Select(x => new OrderLDTO
                {
                    OrderId = x.OrderId,
                    CustomerId = x.CustomerId,
                    CustomerName = x.Customer.Name,
                    Gtotal = x.Gtotal,
                    Status = x.Status,
                    OrderDate = x.OrderDate,
                    CreateAt = x.CreateAt
                }).ToList();
        }

        public static List<OrderLDTO> GetOrdersByStatus(string status)
        {
            using var context = new FoodStoreContext();
            return context.Orders
                .Include(x => x.Customer)
                .Where(x => x.Status != null && x.Status.Trim().ToLower() == status.Trim().ToLower())
                .Select(x => new OrderLDTO
                {
                    OrderId = x.OrderId,
                    CustomerId = x.CustomerId,
                    CustomerName = x.Customer != null ? x.Customer.Name : "-",
                    Gtotal = x.Gtotal,
                    Status = x.Status,
                    OrderDate = x.OrderDate,
                    CreateAt = x.CreateAt
                })
                .ToList();
        }

        public static List<OrderDetailDTO> GetAllOrderDetail()
        {
            using (var context = new FoodStoreContext())
            {
                var orders = context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Cate)
                    .Select(o => new OrderDetailDTO
                    {
                        OrderId = o.OrderId,
                        CustomerId = o.CustomerId,
                        Gtotal = (decimal)o.Gtotal,
                        CreateAt = o.CreateAt,
                        UpdateAt = o.UpdateAt,
                        Status = o.Status,
                        OrderDate = o.OrderDate,
                        OrderItems = o.OrderItems.Select(oi => new OrderItemsDTO
                        {
                            OrderItemId = oi.OrderItemId,
                            OrderId = oi.OrderId,
                            ProductId = oi.ProductId,
                            Quantity = oi.Quantity,
                            CreateAt = oi.CreateAt,
                            UpdateAt = oi.UpdateAt,
                            Name = oi.Product != null ? oi.Product.Name : null,
                            Price = oi.Product != null ? oi.Product.Price : null,
                            Images = oi.Product != null ? oi.Product.Images : null,
                            OriginalPrice = oi.Product != null ? oi.Product.OriginalPrice : null,
                            CategoryName = oi.Product != null && oi.Product.Cate != null ? oi.Product.Cate.CateName : null
                        }).ToList()
                    })
                    .ToList();
                return orders;
            }
        }


    }
}

