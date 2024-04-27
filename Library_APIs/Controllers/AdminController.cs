using Library_APIs.Data;
using Library_APIs.DTO;
using Library_APIs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Library_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LibraryContext _context;
        public AdminController(UserManager<ApplicationUser> userManager , LibraryContext context)
        {
            this._userManager = userManager;
            this._context = context;
        }
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(int page , int limit)
        {
            var TotalCount = await _userManager.Users.CountAsync();
            var totalPages = (int)Math.Ceiling(TotalCount / (double)limit);
            var PagedUsers = await _userManager.Users
                .Skip((page - 1)*limit)
                .Take(limit)
                .ToListAsync();
            var PagedUserResults = new PagedUserResult
            {
                TotalCount = TotalCount,
                TotalPages = totalPages,
                PageNumber = page,
                Users = PagedUsers
            };
            if(PagedUserResults.TotalCount > 0)
            {
                return Ok(PagedUserResults);
            }
            return NotFound("No users has been found");
        }

        [HttpGet("SearchUsers")]
        public async Task<IActionResult> GetSearchedUsers(string? term)
        {
            IQueryable<ApplicationUser> users;
            if (string.IsNullOrWhiteSpace(term))
                users =  _userManager.Users;
            else
            {
                term = term.Trim().ToLower();
                users = _userManager.Users.Where(u => u.UserName.ToLower().Contains(term));
            }

            if (users.Any())
                return Ok(users);
            else
                return NotFound("No users has been found");
        } 

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser(UserDataDTO userDTO)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = userDTO.UserName;
                user.LastName = userDTO.LastName;
                user.FirstName = userDTO.FirstName;
                user.Email = userDTO.Email;
                
                IdentityResult result = await _userManager.CreateAsync(user,userDTO.Password);
                if (result.Succeeded)
                {
                    return Ok("User created successfully");
                }
                return BadRequest(result.Errors.FirstOrDefault());
            }
            return BadRequest(ModelState);
        }

        [HttpGet("GetUser/{id:Guid}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user =  _userManager.Users.FirstOrDefault(u=>u.Id == id);
            if(user != null)
            {
                return Ok(user);
            }
            return NotFound("User does not exist");
        }

        [HttpPut("UpdateUser/{id:Guid}")]
        public async Task<IActionResult> UpdateUser(UserDataDTO userDTO, string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(user => user.Id == id);
            if (ModelState.IsValid)
            {

                if (user != null)
                {
                    user.Email = userDTO.Email;
                    user.FirstName = userDTO.FirstName;
                    user.LastName = userDTO.LastName;
                    user.UserName = userDTO.UserName;
                    _context.Update(user);
                    _context.SaveChanges();
                    return Ok("User updated successfully");
                }
                return NotFound("User is not found");
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("DeleteUser/{id:Guid}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user != null)
            {
               IdentityResult result = await _userManager.DeleteAsync(user);
               if(result.Succeeded)
                {
                    return Ok("User deleted successfully");
                }
                return BadRequest(result.Errors.FirstOrDefault());
            }
            return NotFound();
        }
        [HttpGet("GetPendingBooks")]
        public async Task<IActionResult> GetPendingBooks()
        {
            List<Book> books = await _context.Books.Where(b=>b.BookState == BookState.Pending).ToListAsync();
            if(books != null)
            {
                return Ok(books);
            }
            return NotFound("No pending requests");
        }

        [HttpPut("ApproveRequest/{id:Guid}")]
        public async Task<IActionResult> ApproveRequest(Guid id)
        {
            Book? book = await _context.Books.FindAsync(id);
            if(book != null)
            {
                if(book.BookState == BookState.Pending)
                {
                    book.BookState = BookState.Approved;
                    _context.Update(book);
                    _context.SaveChanges();
                }
            }
            return NotFound("Book is not found");
        }

        [HttpDelete("RejectRequest/{id:Guid}")]
        public async Task<IActionResult> RejectRequest(Guid id)
        {
            Book? book = await _context.Books.FindAsync(id);
            if(book != null)
            {
                if(book.BookState == BookState.Pending)
                {
                    book.BookState = BookState.Rejected;
                    _context.Update(book);
                    _context.SaveChanges();
                }

            }
            return NotFound("Book is not found");
        }

        [HttpGet("GetRejectedReasons")]
        public  IActionResult GetRejectedReasons()
        {
            var reasons = Enum.GetValues(typeof(RejectReasons))
                              .Cast<RejectReasons>()
                              .Select(e => new {Name = e.ToString() , Value = (int)e })
                              .ToList();

            if(reasons != null)
            {
                return Ok(reasons);
            }
            return BadRequest("No reasons");
        }
    }
}
