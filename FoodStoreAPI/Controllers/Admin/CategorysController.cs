using AutoMapper;
using FoodStoreAPI.DTO;
using FoodStoreAPI.DTOs;
using FoodStoreAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodStoreAPI.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategorysController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly FoodStoreContext _context;
        public CategorysController(IMapper mapper, FoodStoreContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("Admin/Categorys")]
        public ActionResult<IEnumerable<CategoryDTO>> GetCategory([FromQuery] string? search, [FromQuery] string? sort)
        {
            try
            {
                var query = _context.Categories.AsQueryable();

                // Tìm kiếm nếu có từ khóa và khác "all"
                if (!string.IsNullOrWhiteSpace(search) && search.ToLower() != "all")
                {
                    query = query.Where(x => x.CateName.ToLower().Contains(search.ToLower()));
                }

                // Sắp xếp nếu có yêu cầu và khác "all"
                if (!string.IsNullOrWhiteSpace(sort) && sort.ToLower() != "all")
                {
                    if (sort.ToLower() == "asc")
                        query = query.OrderBy(x => x.CateName);
                    else if (sort.ToLower() == "desc")
                        query = query.OrderByDescending(x => x.CateName);
                }

                var categories = query.Select(x => new CategoryDTO
                {
                    CateId = x.CateId,
                    CateName = x.CateName
                }).ToList();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("Admin/Categorys/{id}")]
        public ActionResult GetCategoryById(int id)
        {
            try
            {
                var category = _context.Categories.Include(x => x.Products)
                    .Where(x => x.CateId == id).FirstOrDefault();
                if(category == null)
                {
                    return BadRequest("Category is not found");
                }
                var categoryDetails = _mapper.Map<CategoryDetailsDTO>(category);
                return Ok(categoryDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Admin/AddCategory")]
        public IActionResult AddCategory([FromBody] CategoryDTO categoryDTO)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest("ModelState Error: " + string.Join(" | ", errorList));
            }
            //Check null or empty
            if (string.IsNullOrEmpty(categoryDTO.CateName))
            {
                return BadRequest("CategoryName is not null!");
            }
            try
            {
                bool categoryExisted = _context.Categories.Any(x => x.CateName.Equals(categoryDTO.CateName));
                if (categoryExisted)
                {
                    return BadRequest("Category is existed!");
                }
                
                var category = _mapper.Map<Category>(categoryDTO);
                _context.Categories.Add(category);
                _context.SaveChanges();
                return Ok("Add new category successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Admin/UpdateCategory/{id}")]
        public IActionResult UpdateCategory(int id, [FromBody] CategoryDTO categoryDTO)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest("ModelState Error: " + string.Join(" | ", errorList));
            }
            
            try
            {
                var category = _context.Categories.Find(id);
                if (category == null)
                {
                    return NotFound("Category not found");
                }
                if (string.IsNullOrEmpty(categoryDTO.CateName))
                {
                    return BadRequest("CategoryName is not null!");
                }
                bool categoryExisted = _context.Categories.Any(x => x.CateName.Equals(categoryDTO.CateName));
                if (categoryExisted)
                {
                    return BadRequest("Category is existed!");
                }
                category.CateName = categoryDTO.CateName;
                var categoryUpdate = _mapper.Map<Category>(category);
                _context.Categories.Update(categoryUpdate);
                _context.SaveChanges();
                return Ok("Update category successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Admin/DeleteCategory/{id}")]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                var category = _context.Categories.Find(id);
                if (category == null)
                {
                    return NotFound("Category not found");
                }
                _context.Categories.Remove(category);
                _context.SaveChanges();
                return Ok("Delete category successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
