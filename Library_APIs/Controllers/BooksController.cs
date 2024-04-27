using Library_APIs.Data;
using Library_APIs.DTO;
using Library_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Identity;
using System.Runtime.InteropServices;

namespace Library_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryContext db;
        private readonly IWebHostEnvironment _environment;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager;

        public BooksController(LibraryContext _db, IWebHostEnvironment environment, UserManager<ApplicationUser> userManager)
        {
            db = _db;
            this._environment = environment;
            this.userManager = userManager;
        }

        [HttpGet("GetAllBooks")]
        public async Task<IActionResult> GetAllBooks(int page,int limit)
        {
            var books = db.Books;

            var totalCount = await db.Books.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)limit);
            var pagedBooks = await books.Include(b => b.Category).Skip((page -1) * limit) .Take
                (limit).ToListAsync();

            var pagedBookData = new PagedBookResult
            {
                books = pagedBooks,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            if (pagedBookData.TotalCount < 1)
            {
                return NotFound("There are no books found");

            }
            return Ok(pagedBookData);


        }

        [HttpGet("SearchBook")]
        public IActionResult GetSearchedBook(string term) 
        {
            IQueryable<Book> books;
            if (string.IsNullOrWhiteSpace(term))
                books = db.Books;
            else
            {
                term = term.Trim().ToLower();
                books = db.Books
                        .Where(b => b.Title.ToLower().Contains(term)
                        );
            }
            if (books.Any())
                return Ok(books);

            else
                return NotFound("No books matches the search term");

        }

        [HttpPost("CreateBook")]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateBook(BookWithCategoryDTO bookDTO)
        {
            var book = new Book();
            if (bookDTO != null)
            {
                if (ModelState.IsValid)
                {
                    book.Id = Guid.NewGuid();
                    book.Title = bookDTO.Title;
                    book.Description = bookDTO.Description;
                    book.CategoryId = bookDTO.CategoryId;

                    string wwwRootPath = _environment.WebRootPath;

                    if (bookDTO.Image != null)
                    {
                        string fileName = Guid.NewGuid().ToString();
                        var uploads = Path.Combine(wwwRootPath, @"images\Books");
                        var extension = Path.GetExtension(bookDTO.Image.FileName);

                        if (book.Image != null)
                        {
                            var oldImagePath = Path.Combine(wwwRootPath, book.Image.TrimStart('\\'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        using (
                            var fileStream = new FileStream(
                                Path.Combine(uploads, fileName + extension),
                                FileMode.Create)
                            )
                        {
                            bookDTO.Image.CopyTo(fileStream);
                        }
                        book.Image = @"images\Books\" + fileName + extension;

                        db.Books.Add(book);
                        db.SaveChanges();
                        return Ok(book);
                    }

                }
            }
            return BadRequest("Model not valid");

        }

        [HttpGet("GetBook/{id}")]
        public IActionResult GetBook(Guid id)
        {
            var book = db.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound("Book not found");
            }
            return Ok(book);
        }

        [HttpPost("UpdateBook/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateBook(Guid id, BookWithCategoryDTO bookDTO)
        {
            var book = db.Books.Include(b=>b.Category).FirstOrDefault(b=>b.Id == id);

            if (book == null)
            { return NotFound("Book not found"); }

            book.Title = bookDTO.Title;
            book.Description = bookDTO.Description;
            book.CategoryId = bookDTO.CategoryId;

            string wwwRootPath = _environment.WebRootPath;

            if (bookDTO.Image != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\Books");
                var extension = Path.GetExtension(bookDTO.Image.FileName);

                if (book.Image != null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath, book.Image.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (
                    var fileStream = new FileStream(
                        Path.Combine(uploads, fileName + extension),
                        FileMode.Create)
                    )
                {
                    bookDTO.Image.CopyTo(fileStream);
                }
                book.Image = @"images\Books\" + fileName + extension;

            }
            db.Books.Update(book);
            db.SaveChanges();
            return Ok(book);

        }

        [HttpDelete("DeleteBook/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteBook (Guid id)
        {
            var book = db.Books.Find(id);
            if(book == null)
            { return NotFound("Book not found"); }

            db.Books.Remove(book);
            db.SaveChanges();
            return Ok("Deleted successfully");
        }

        [HttpPost("SendRequest/{id}")]
        public async Task<IActionResult> SendRequest(Guid id)
        {
            var userId = userManager.GetUserId(HttpContext.User);
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            var book = db.Books.Find(id);

            if (book == null)
                return NotFound("Book not found");

            var request = new Request();
            book.BookState = BookState.Pending;
            request.Id = Guid.NewGuid();
            request.UserId = userId;
            request.BookeState = BookState.Pending;
            request.BookId = id;
            db.Add(request);
            db.SaveChanges();
            return Ok();
        }
    }
}
