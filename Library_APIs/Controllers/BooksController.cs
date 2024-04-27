using Library_APIs.Data;
using Library_APIs.DTO;
using Library_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Library_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryContext db;
        private readonly IWebHostEnvironment _environment;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager;
        private readonly IHttpContextAccessor httpContext;

        public BooksController(LibraryContext _db, 
                                IWebHostEnvironment environment, 
                                UserManager<ApplicationUser> userManager,
                                IHttpContextAccessor httpContext)
        {
            db = _db;
            this._environment = environment;
            this.userManager = userManager;
            this.httpContext = httpContext;
        }

        [HttpGet("GetAllBooks")]
        public async Task<IActionResult> GetAllBooks(int page,int limit)
        {
            var books = db.Books;

            var totalCount = await db.Books.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)limit);
            var pagedBooks = await books.Skip((page -1) * limit) .Take
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
            var bookList = new List<string>();
            var termSession="";
            var result = "";
            if (string.IsNullOrWhiteSpace(term))
                books = db.Books;
            else
            {
                term = term.Trim().ToLower();
                books = db.Books
                        .Where(b => b.Title.ToLower().Contains(term)
                        );
                bookList.Add(term);
                termSession = JsonConvert.SerializeObject(bookList);
                httpContext.HttpContext.Session.SetString("BookList",termSession);
               
            }

            if (books.Any())
                return Ok(books);

            else
                return NotFound("No books matches the search term " );

        }

        [HttpGet("GetSerchHistory")]
        public IActionResult GetSearchHistory()
        {
            List<string> bookList = new List<string>();

            string bookSession = httpContext.HttpContext.Session.GetString("BookList");
            if(!string.IsNullOrWhiteSpace(bookSession))
                bookList = JsonConvert.DeserializeObject<List<string>>(bookSession);

            if(bookList.Any())
                return Ok(bookList);

            return Ok();
        }
        

        [HttpPost("CreateBook")]
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
