using Library_APIs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Library_APIs.Data
{
    public class LibraryContext : IdentityDbContext<ApplicationUser>
    {
        public LibraryContext() { }
        public LibraryContext(DbContextOptions options):base(options) { }
        public DbSet<ApplicationUser> Users {get; set;}
        public DbSet<Book> Books { get; set;}
        public DbSet<Category> Categories { get; set;}
        public DbSet<Request> Requests { get; set; }
    }
}
