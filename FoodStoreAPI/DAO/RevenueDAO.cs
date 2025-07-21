using FoodStoreAPI.DTO;
using FoodStoreAPI.Models;

namespace FoodStoreAPI.DAO
{
    public class RevenueDAO
    {

        public static Revenue findRevenue(int accId)
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);
            FoodStoreContext context = new FoodStoreContext();
            Revenue revenue = new Revenue();
            try
            {
                revenue = context.Revenues.FirstOrDefault(x => x.AccId == accId && x.CreateAt >= today && x.CreateAt < tomorrow);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return revenue;
        }
        public static List<RevenueDTO> getRevenue(int accId, DateTime dateNow, DateTime dateSevenDay)
        {
            FoodStoreContext context = new FoodStoreContext();
            List<RevenueDTO> revenue = new List<RevenueDTO>();
            try
            {
                revenue = context.Revenues.Where(x => x.CreateAt >= dateSevenDay && x.CreateAt <= dateNow && x.AccId == accId)
                    .Select(x => new RevenueDTO
                    {
                        RevenueId = x.RevenueId,
                        RevenuePrice = x.RevenuePrice,
                        InterestRate = x.InterestRate,
                        FloorFee = x.FloorFee,
                        CreateAt = x.CreateAt,
                        AccId = x.AccId
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return revenue;
        }
      
    
    public static List<RevenueDTO> getRevenueAdmin(DateTime dateNow, DateTime dateSevenDay)
        {
            FoodStoreContext context = new FoodStoreContext();
            List<RevenueDTO> revenue = new List<RevenueDTO>();
            try
            {
                revenue = context.Revenues.Where(x => x.CreateAt >= dateSevenDay && x.CreateAt <= dateNow)
                    .Select(x => new RevenueDTO
                    {
                        RevenueId = x.RevenueId,
                        RevenuePrice = x.RevenuePrice,
                        InterestRate = x.InterestRate,
                        FloorFee = x.FloorFee,
                        CreateAt = x.CreateAt,
                        AccId = x.AccId
                    })
                    .ToList();
            } 
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return revenue;
        }
    }
}
