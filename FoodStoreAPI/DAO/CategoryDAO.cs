using AutoMapper;
using FoodStoreAPI.DTO;
using FoodStoreAPI.Models;

namespace FoodStoreAPI.DAO
{
    public class CategoryDAO
    {

        public static List<Category> getCategory()
        {
            List<Category> listCate = new List<Category>();
            try
            {
                listCate = FoodStoreContext.Ins.Categories.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listCate;
        }
        public static List<CategoryDTO> getCategoryDTO()
        {
            List<CategoryDTO> listCate = new List<CategoryDTO>();
            try
            {
                listCate = FoodStoreContext.Ins.Categories.Select(x => new CategoryDTO
                {
                    CateId = x.CateId,
                    CateName = x.CateName
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listCate;
        }

        //Create Category
        public static bool AddCategory(Category category)
        {
            try
            {
                var categoryExist = FoodStoreContext.Ins.Categories.FirstOrDefault(x => x.CateName.Equals(category.CateName));
                if (categoryExist != null)
                {
                    return false;
                }
                Category cate = new Category
                {
                    CateName = category.CateName
                };
                FoodStoreContext.Ins.Categories.Add(cate);
                FoodStoreContext.Ins.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Update Category
        public static bool UpdateCategory(Category category)
        {
            try
            {
                var categoryExist = FoodStoreContext.Ins.Categories.FirstOrDefault(x => x.CateId == category.CateId);
                if (categoryExist == null)
                {
                    return false;
                }
                categoryExist.CateName = category.CateName;
                FoodStoreContext.Ins.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Delete Category
        public static bool DeleteCategory(int id)
        {
            try
            {
                var categoryExist = FoodStoreContext.Ins.Categories.FirstOrDefault(x => x.CateId == id);
                if (categoryExist == null)
                {
                    return false;
                }
                FoodStoreContext.Ins.Categories.Remove(categoryExist);
                FoodStoreContext.Ins.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Get Category by ID
        public static Category getCategoryById(int id)
        {
            try
            {
                var categoryExist = FoodStoreContext.Ins.Categories.FirstOrDefault(x => x.CateId == id);
                if (categoryExist == null)
                {
                    return null;
                }
                return categoryExist;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }        
    }
}
