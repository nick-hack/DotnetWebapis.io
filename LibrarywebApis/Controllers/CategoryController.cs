using LibrarywebApis.Models;
using LibrarywebApis.Models.Model;
using LiraryManagement.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext appDBContext;

        public CategoryController(AppDbContext appDBContext)
        {
            this.appDBContext = appDBContext;
        }

        // GET: api/category (for pagination)
        [HttpGet("paginated-categories")]
        public async Task<IActionResult> GetPaginatedCategories(int page = 1, int pageSize = 10)
        {
            // Total number of categories
            int totalCategories = await appDBContext.Categories.CountAsync();

            // Calculate the number of records to skip
            int skipCount = (page - 1) * pageSize;

            // Get paginated active categories
            var paginatedCategories = await appDBContext.Categories
                                          .Where(c => c.IsActive)
                                          .Skip(skipCount)
                                          .Take(pageSize)
                                          .ToListAsync();

            // Prepare the response
            var response = new
            {
                Categories = paginatedCategories.Select(c => new
                {
                    c.Id,
                    c.CategoryName,
                    c.IsActive
                }),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalCategories / pageSize)
            };

            return Ok(response);
        }

        // POST: api/category (add category)
        [HttpPost("add")]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryModel addCategoryModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = new Category
            {
                CategoryName = addCategoryModel.CategoryName,
                IsActive = true
            };

            await appDBContext.Categories.AddAsync(category);
            await appDBContext.SaveChangesAsync();

            return Ok(new { Message = "Category added successfully." });
        }

        // GET: api/category/{id} (get category by ID)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await appDBContext.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new { Message = "Category not found." });
            }

            var categoryDetails = new
            {
                category.Id,
                category.CategoryName,
                category.IsActive
            };

            return Ok(categoryDetails);
        }

        // PUT: api/category/edit (edit category)
        [HttpPut("edit")]
        public async Task<IActionResult> EditCategory([FromBody] UpdateCategoryModel updateCategoryModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await appDBContext.Categories.FindAsync(updateCategoryModel.Id);

            if (category == null)
            {
                return NotFound(new { Message = "Category not found." });
            }

            category.CategoryName = updateCategoryModel.CategoryName;
            category.IsActive = updateCategoryModel.IsActive;

            await appDBContext.SaveChangesAsync();

            return Ok(new { Message = "Category updated successfully." });
        }

        // DELETE: api/category/delete (delete category)
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteCategory([FromBody] UpdateCategoryModel updateCategoryModel)
        {
            var category = await appDBContext.Categories.FindAsync(updateCategoryModel.Id);

            if (category == null)
            {
                return NotFound(new { Message = "Category not found." });
            }

            appDBContext.Categories.Remove(category);
            await appDBContext.SaveChangesAsync();

            return Ok(new { Message = "Category deleted successfully." });
        }
    }
}
