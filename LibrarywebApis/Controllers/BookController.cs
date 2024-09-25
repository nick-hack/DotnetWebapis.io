using LibrarywebApis.Models;
using LibrarywebApis.Models.Model;
using LiraryManagement.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace LibrarywebApis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly AppDbContext appDBContext;

        public BookController(AppDbContext appDBContext)
        {
            this.appDBContext = appDBContext;
        }

        [HttpGet("paginated-books")]
        public async Task<IActionResult> GetPaginatedBooks(string searchTitle = null, string searchCategory = null, string searchAuthor = null, int page = 1, int pageSize = 10)
        {
            var booksQuery = appDBContext.Books
                .Include(b => b.Categories)
                .Include(b => b.Authors)
                .Where(b => b.IsActive) // Assuming there is an IsActive property
                .OrderByDescending(b => b.PublishedDate)
                .AsQueryable();

            // Apply search filters
            if (!string.IsNullOrEmpty(searchTitle))
            {
                booksQuery = booksQuery.Where(b => b.Title.Contains(searchTitle));
            }

            if (!string.IsNullOrEmpty(searchCategory))
            {
                booksQuery = booksQuery.Where(b => b.Categories.CategoryName.Contains(searchCategory));
            }

            if (!string.IsNullOrEmpty(searchAuthor))
            {
                booksQuery = booksQuery.Where(b => b.Authors.Name.Contains(searchAuthor));
            }

            // Total number of books after filtering
            int totalBooks = await booksQuery.CountAsync();

            // Pagination
            int skipCount = (page - 1) * pageSize;
            var paginatedBooks = await booksQuery.Skip(skipCount).Take(pageSize).ToListAsync();

            var response = new
            {
                Books = paginatedBooks.Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.ISBN,
                    b.PublishedDate,
                    CategoryName = b.Categories.CategoryName,
                    AuthorName = b.Authors.Name
                }),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalBooks / pageSize),
                SearchTitle = searchTitle,
                SearchCategory = searchCategory,
                SearchAuthor = searchAuthor
            };

            return Ok(response);
        }


        [HttpGet("add")]
        public async Task<IActionResult> GetCategoriesAndAuthors()
        {
            var categories = await appDBContext.Categories
                .Select(c => new { c.Id, c.CategoryName })
                .ToListAsync();

            var authors = await appDBContext.Authors
                .Select(a => new { a.Id, a.Name })
                .ToListAsync();

            return Ok(new { Categories = categories, Authors = authors });
        }


        [HttpPost("add")]
        public async Task<IActionResult> AddBook([FromBody] AddBookModel addBookModel)
        {
            var existingBook = await appDBContext.Books
                                  .FirstOrDefaultAsync(b => b.ISBN == addBookModel.ISBN);

            if (existingBook != null)
            {
                return Conflict(new { Message = "A book with this ISBN already exists." });
            }

            var book = new Book
            {
                Title = addBookModel.Title,
                ISBN = addBookModel.ISBN,
                PublishedDate = addBookModel.PublishedDate,
                CategoriesId = addBookModel.CategoriesId,
                AuthorsId = addBookModel.AuthorsId,
                IsActive = true
            };

            await appDBContext.Books.AddAsync(book);
            await appDBContext.SaveChangesAsync();

            return Ok(new { Message = "Book added successfully." });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await appDBContext.Books
                            .Include(b => b.Categories)
                            .Include(b => b.Authors)
                            .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound(new { Message = "Book not found." });
            }

            var bookDetails = new
            {
                book.Id,
                book.Title,
                book.ISBN,
                book.PublishedDate,
                CategoryName = book.Categories.CategoryName,
                AuthorName = book.Authors.Name
            };

            return Ok(bookDetails);
        }


        [HttpPost("edit")]
        public async Task<IActionResult> UpdateBook([FromBody] UpdateBookModel updateBookModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var book = await appDBContext.Books.FindAsync(updateBookModel.Id);

            if (book == null)
            {
                return NotFound(new { Message = "Book not found." });
            }

            book.Title = updateBookModel.Title;
            book.ISBN = updateBookModel.ISBN;
            book.PublishedDate = updateBookModel.PublishedDate;

            await appDBContext.SaveChangesAsync();

            return Ok(new { Message = "Book updated successfully." });
        }


        [HttpPost("delete")]
        public async Task<IActionResult> DeleteBook([FromBody] UpdateBookModel updateBookModel)
        {
            var book = await appDBContext.Books.FindAsync(updateBookModel.Id);

            if (book == null)
            {
                return NotFound(new { Message = "Book not found." });
            }

            appDBContext.Books.Remove(book);
            await appDBContext.SaveChangesAsync();

            return Ok(new { Message = "Book deleted successfully." });
        }



    }


}