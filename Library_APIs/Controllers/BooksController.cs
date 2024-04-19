using Library_APIs.Data;
using Library_APIs.DTO;
using Library_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace Library_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryContext db;
        private readonly IWebHostEnvironment _environment;

        public BooksController(LibraryContext _db, IWebHostEnvironment environment)
        {
            db = _db;
            this._environment = environment;
        }

        [HttpGet("GetAllBooks")]
        public IActionResult GetAllRecipes()
        {
            var books = db.Books.ToList();
            if (books.Any())
            {
                return Ok(books);
            }
            return NotFound("There are no books found");

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
                    book.Category = bookDTO.Category;

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
            var book = db.Books.Find(id);

            if (book == null)
            { return NotFound("Book not found"); }

            book.Title = bookDTO.Title;
            book.Description = bookDTO.Description;
            book.Category = bookDTO.Category;

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
    }
}
