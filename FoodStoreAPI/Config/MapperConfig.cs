using AutoMapper;
using FoodStoreAPI.DAO;
using FoodStoreAPI.DTO;
using FoodStoreAPI.DTOs;
using FoodStoreAPI.Models;

namespace FoodStoreAPI.Config
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Account, AccountDAO>().ReverseMap();
            CreateMap<Account, AccountLDTO>().ReverseMap();
            CreateMap<AccountLDTO, Account>().ReverseMap();

           

            CreateMap<CategoryDTO, Category>().ReverseMap();
            CreateMap<Product, ProductByCategoryDTO>().ReverseMap();
            CreateMap<Category, CategoryDetailsDTO>().ForMember(opt => opt.Products, dest => dest.MapFrom(x => x.Products)).ReverseMap();
        }
    }
}
