using FoodStoreAPI.DTO;
using FoodStoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodStoreAPI.DAO
{
    public class ProductDAO
    {

        public static List<Product> GetProduct(int categoryId)
        {
            var products = new List<Product>();
            try
            {
                FoodStoreContext context = new FoodStoreContext();
                products = context.Products.Where(x => x.CateId == categoryId && x.ProductStatus == "On").ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return products;
        }

        public static List<Product> getProduct(int categoryId)
        {
            var listProduct = new List<Product>();
            try
            {
                FoodStoreContext context = new FoodStoreContext();
                listProduct = context.Products.Where(x => x.CateId == categoryId && x.ProductStatus == "On").ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listProduct;
        }
        public static List<ProductDTO> GetProductOn()
        {
            var listProduct = new List<ProductDTO>();
            try
            {
                FoodStoreContext context = new FoodStoreContext();
                listProduct = context.Products.Include(x => x.Cate)
                                              .Include(x => x.Acc)
                                              .Where(x => x.ProductStatus == "On")
                                              .Select(x => new ProductDTO
                                              {
                                                  ProId = x.ProId,
                                                  Name = x.Name,
                                                  Price = x.Price,
                                                  Unit = x.Unit,
                                                  Images = x.Images,
                                                  CreateAt = x.CreateAt,
                                                  Quantity = x.Quantity,
                                                  ProductStatus = x.ProductStatus,
                                                  AccId = x.AccId,
                                                  CateId = x.CateId,
                                              }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listProduct;

        }

            public static bool saveProduct(ProductDTO pro)
            {
                try
                {
                    Category category = FoodStoreContext.Ins.Categories.SingleOrDefault(x => x.CateId == pro.CateId);
                    Account account = FoodStoreContext.Ins.Accounts.SingleOrDefault(x => x.AccId == pro.AccId);

                    if (category != null && account != null)
                    {
                        Product product = new Product
                        {
                            Name = pro.Name,
                            Price = pro.Price,
                            OriginalPrice = pro.OriginalPrice,
                            Unit = pro.Unit,
                            Images = pro.Images,
                            Quantity = pro.Quantity,
                            ProductStatus = pro.ProductStatus,
                            AccId = pro.AccId,
                            CateId = pro.CateId,
                            DiscountStartTime = pro.DiscountStartTime,
                            DiscountEndTime = pro.DiscountEndTime,
                            CreateAt = DateTime.Now
                        };
                        FoodStoreContext.Ins.Products.Add(product);
                        int rows = FoodStoreContext.Ins.SaveChanges();
                        return rows > 0;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fail to add Product");
                    return false;
                }
            }


            public static ProductDTO findProductById(int proId)
            {
                FoodStoreContext context = new FoodStoreContext();
                var pro = new ProductDTO();
                try
                {
                    pro = context.Products
                        .Include(x => x.Cate)
                        .Include(x => x.Acc)
                        .Where(x => x.ProId == proId)
                        .Select(x => new ProductDTO
                        {
                            ProId = x.ProId,
                            Images = x.Images,
                            Name = x.Name,
                            Price = x.Price,
                            Unit = x.Unit,
                            Quantity = x.Quantity,
                            ProductStatus = x.ProductStatus,
                            CreateAt = x.CreateAt,
                            CateId = x.CateId,
                            CateName = x.Cate.CateName,
                            NameAccount = x.Acc.Name
                        })
                        .FirstOrDefault();

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return pro;
            }

            public static void deteleProduct(int proId)
            {
                FoodStoreContext context = new FoodStoreContext();
                try
                {
                    Product product = context.Products.SingleOrDefault(x => x.ProId == proId);
                    product.UpdateAt = DateTime.Now;
                    product.ProductStatus = "Off";
                    context.Products.Update(product);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            public static void updateProductById(int proId, ProductDTO productdto)
            {
                Product pro = FoodStoreContext.Ins.Products.FirstOrDefault(x => x.ProId == proId);
                if (pro != null)
                {
                    try
                    {
                        pro.Name = productdto.Name;
                        pro.Price = productdto.Price;
                        pro.OriginalPrice = productdto.OriginalPrice;
                        pro.Unit = productdto.Unit;
                        pro.Images = productdto.Images;
                        pro.Quantity = productdto.Quantity;
                        pro.ProductStatus = productdto.ProductStatus = productdto.ProductStatus;
                        pro.AccId = productdto.AccId;
                        pro.CateId = productdto.CateId;
                        pro.DiscountStartTime = productdto.DiscountStartTime;
                        pro.DiscountEndTime = productdto.DiscountEndTime;
                        pro.UpdateAt = DateTime.Now;
                        FoodStoreContext.Ins.Products.Update(pro);
                        FoodStoreContext.Ins.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }

            public static List<ProductDTO> getProductByCate(int categoryId)
            {
                var listProduct = new List<ProductDTO>();
                try
                {
                    FoodStoreContext context = new FoodStoreContext();
                    listProduct = context.Products
                        .Include(x => x.Cate)
                        .Include(x => x.Acc)
                        .Where(x => x.CateId == categoryId)
                        .Select(x => new ProductDTO
                        {
                            ProId = x.ProId,
                            Images = x.Images,
                            Name = x.Name,
                            Price = x.Price,
                            Unit = x.Unit,
                            Quantity = x.Quantity,
                            ProductStatus = x.ProductStatus,
                            CateName = x.Cate.CateName,
                            CreateAt = x.CreateAt,
                            NameAccount = x.Acc.Name

                        }).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return listProduct;
            }

            public static List<ProductDTO> getProductOrder(int id)
            {
                var listProduct = new List<ProductDTO>();
                try
                {
                    FoodStoreContext context = new FoodStoreContext();
                    listProduct = context.Products
                        .Include(x => x.Cate)
                        .Include(x => x.Acc)
                        .Where(x => x.Acc.AccId == id)
                        .Select(x => new ProductDTO
                        {
                            Images = x.Images,
                            Name = x.Name,
                            Price = x.Price,
                            Unit = x.Unit,
                            Quantity = x.Quantity,
                            ProductStatus = x.ProductStatus,
                            CateName = x.Cate.CateName,
                            NameAccount = x.Acc.Name

                        }).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return listProduct;
            }

            public static List<ProductDTO> getProductOn()
            {
                var listProduct = new List<ProductDTO>();
                try
                {
                    FoodStoreContext context = new FoodStoreContext();
                    listProduct = context.Products
                        .Include(x => x.Cate)
                        .Include(x => x.Acc)
                        .Where(x => x.ProductStatus == "On")
                        .Select(x => new ProductDTO
                        {
                            ProId = x.ProId,
                            Name = x.Name,
                            Price = x.Price,
                            Unit = x.Unit,
                            Images = x.Images,
                            CreateAt = x.CreateAt,
                            Quantity = x.Quantity,
                            ProductStatus = x.ProductStatus,
                            AccId = x.AccId,
                            CateId = x.CateId,
                            CateName = x.Cate != null ? x.Cate.CateName : null,
                            NameAccount = x.Acc != null ? x.Acc.Name : null
                        }).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return listProduct;
            }


            public static List<ProductDTO> getAllProduct()
            {
                var listProduct = new List<ProductDTO>();
                try
                {
                    FoodStoreContext context = new FoodStoreContext();
                    listProduct = context.Products
                        .Include(x => x.Cate)
                        .Include(x => x.Acc)
                        .Select(x => new ProductDTO
                        {
                            ProId = x.ProId,
                            Name = x.Name,
                            Price = x.Price,
                            Unit = x.Unit,
                            Images = x.Images,
                            CreateAt = x.CreateAt,
                            Quantity = x.Quantity,
                            ProductStatus = x.ProductStatus,
                            AccId = x.AccId,
                            CateId = x.CateId,
                            CateName = x.Cate != null ? x.Cate.CateName : null,
                            NameAccount = x.Acc != null ? x.Acc.Name : null
                        }).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return listProduct;
            }

        // Tìm kiếm sản phẩm theo tên
        public static List<ProductDTO> SearchProducts(string keyword)
        {
            try
            {
                FoodStoreContext context = new FoodStoreContext();
                var products = context.Products
                    .Include(x => x.Cate)
                    .Include(x => x.Acc)
                    .Where(x => x.Name.Contains(keyword))
                    .Select(x => new ProductDTO
                    {
                        ProId = x.ProId,
                        Images = x.Images,
                        Name = x.Name,
                        Price = x.Price,
                        Unit = x.Unit,
                        Quantity = x.Quantity,
                        ProductStatus = x.ProductStatus,
                        CreateAt = x.CreateAt,
                        CateName = x.Cate.CateName,
                        NameAccount = x.Acc.Name
                    })
                    .ToList(); // chỉ cần ToList() ở đây thôi, không dùng FirstOrDefault

                return products;
            }
            catch (Exception ex)
            {
                // Nếu bạn muốn an toàn hơn, nên log lỗi
                throw;
            }
        }





        public static List<CategoryDTO> getCate()
        {
            List<CategoryDTO> categories = new List<CategoryDTO>();
            try
            {
                FoodStoreContext context = new FoodStoreContext();

                categories = context.Categories.Select(x => new CategoryDTO
                {
                    CateId = x.CateId,
                    CateName = x.CateName
                })
                .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return categories;
        }

        // Sắp xếp sản phẩm theo ProductName hoặc ProductStatus
        public static List<ProductDTO> OrderProducts(string orderByField, bool asc = true)
            {
                try
                {
                    FoodStoreContext context = new FoodStoreContext();
                    IQueryable<Product> query = context.Products.Include(x => x.Cate).Include(x => x.Acc);

                    switch (orderByField)
                    {
                        case "ProductName":
                            query = asc ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name);
                            break;
                        case "Status":
                            query = asc ? query.OrderBy(x => x.ProductStatus) : query.OrderByDescending(x => x.ProductStatus);
                            break;
                        default:
                            query = asc ? query.OrderBy(x => x.ProId) : query.OrderByDescending(x => x.ProId);
                            break;
                    }

                    var products = query
                        .Select(x => new ProductDTO
                        {
                            ProId = x.ProId,
                            Images = x.Images,
                            Name = x.Name,
                            Price = x.Price,
                            Unit = x.Unit,
                            Quantity = x.Quantity,
                            ProductStatus = x.ProductStatus,
                            CreateAt = x.CreateAt,
                            CateName = x.Cate.CateName,
                            NameAccount = x.Acc.Name
                        }).ToList();

                    return products;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }            
            }
            public static bool AddNewProduct(ProductDTO productDTO)
            {
                FoodStoreContext context = new FoodStoreContext();
                try
                {
                    Product product = new Product
                    {
                        Name = productDTO.Name,
                        Price = productDTO.Price,
                        Unit = productDTO.Unit,
                        OriginalPrice = productDTO.OriginalPrice,
                        Quantity = productDTO.Quantity,
                        ProductStatus = productDTO.ProductStatus,
                        CateId = productDTO.CateId,
                        Images = productDTO.Images,
                        CreateAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        AccId = 4
                    };
                    context.Products.Add(product);
                    context.SaveChanges();
                return true;    
                }               
                catch (Exception e)
                {
                   return false;
                }
            }
            public static void UpdateProduct(int proId, productLDTO productDTO)
            {
                FoodStoreContext context = new FoodStoreContext();
                try
                {
                    Product pro = context.Products.FirstOrDefault(x => x.ProId == proId);
                    pro.Name = productDTO.Name;
                    pro.Price = productDTO.Price;
                    pro.Unit = productDTO.Unit;
                    pro.Quantity = productDTO.Quantity;
                    pro.ProductStatus = productDTO.ProductStatus;
                    pro.CateId = productDTO.CateId;
                    pro.Images = productDTO.Images;
                    pro.UpdateAt = DateTime.Now;
                    context.Products.Update(pro);
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
            public static ProductDTO findProductByLastList(int accId)
            {
                FoodStoreContext context = new FoodStoreContext();
                var pro = new ProductDTO();
                try
                {
                    pro = context.Products
                        .Include(x => x.Cate)
                                    .Where(x => x.AccId == accId)
                                    .OrderBy(x => x.ProId)
                                    .Select(o => new ProductDTO
                                    {
                                        Images = o.Images,
                                        Price = o.Price,
                                        Unit = o.Unit,
                                        Quantity = o.Quantity,
                                        ProductStatus = o.ProductStatus,
                                        CreateAt = o.CreateAt,
                                        CateName = o.Cate.CateName,
                                        NameAccount = o.Acc.Name,
                                        ProId = o.ProId
                                    })
                                    .LastOrDefault();

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return pro;
            }
        }
    }






