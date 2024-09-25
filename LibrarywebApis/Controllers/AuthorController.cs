using LibrarywebApis.Models;
using LibrarywebApis.Models.Model;
using LiraryManagement.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarywebApis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorController : ControllerBase
    {
            private readonly AppDbContext appDBContext;

            public AuthorController(AppDbContext appDBContext)
            {
                this.appDBContext = appDBContext;
            }



        [HttpGet("paginated-authors")]
        public async Task<IActionResult> GetPaginatedAuthors(int page = 1, int pageSize = 10)
        {
            // Total number of authors
            int totalAuthors = await appDBContext.Authors.CountAsync();

            // Calculate the number of records to skip
            int skipCount = (page - 1) * pageSize;

            // Get paginated authors and count associated books for each author
            var paginatedAuthors = await appDBContext.Authors
                                          .Where(a => a.IsActive)   // Filter only active authors
                                          .Skip(skipCount)
                                          .Take(pageSize)
                                          .Select(author => new
                                          {
                                              Id = author.Id,
                                              Name = author.Name,
                                              IsActive = author.IsActive,
                                              BookCount = appDBContext.Books.Count(b => b.Authors.Id == author.Id)  // Count books for each author
                                          })
                                          .ToListAsync();

            // Return the authors and pagination data as JSON
            var response = new
            {
                Authors = paginatedAuthors,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalAuthors / pageSize)
            };

            return Ok(response);
        }

        [HttpPost("add-author")]
        public async Task<IActionResult> AddAuthor([FromBody] AddAuthorModel addAuthorModel)
        {
            // Check if the author name already exists in the database
            var existingAuthor = await appDBContext.Authors
                                   .FirstOrDefaultAsync(a => a.Name == addAuthorModel.Name);

            if (existingAuthor != null)
            {
                // Name already exists, return a bad request with error message
                return BadRequest(new { message = "An author with this name already exists." });
            }

            // If no duplicate, create a new author
            var author = new Author()
            {
                Name = addAuthorModel.Name,
                DateOfBirth = addAuthorModel.DateOfBirth,
                IsActive = true
            };

            await appDBContext.Authors.AddAsync(author);
            await appDBContext.SaveChangesAsync();

            // Return a success response with the created author details
            return CreatedAtAction(nameof(GetAuthorById), new { id = author.Id }, author);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            var author = await appDBContext.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }


        // PUT: api/authors/edit
        [HttpPut("edit")]
        public async Task<IActionResult> EditAuthor([FromBody] UpdateAuthorModel updateAuthorModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors if the model is invalid
            }

            var author = await appDBContext.Authors.FindAsync(updateAuthorModel.Id);

            if (author == null)
            {
                return NotFound(new { message = "Author not found." }); // Return 404 if author doesn't exist
            }

            // Update the author details
            author.Name = updateAuthorModel.Name;
            author.DateOfBirth = updateAuthorModel.DateOfBirth;
            //author.IsActive = updateAuthorModel.IsActive;

            await appDBContext.SaveChangesAsync();

            // Return success response with updated author details
            return Ok(author);
        }

        // DELETE: api/authors/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await appDBContext.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound(new { message = "Author not found." }); // Return 404 if author doesn't exist
            }

            // Remove the author from the database
            appDBContext.Authors.Remove(author);
            await appDBContext.SaveChangesAsync();

            // Return success response after deletion
            return Ok(new { message = "Author deleted successfully." });
        }
    }








}


