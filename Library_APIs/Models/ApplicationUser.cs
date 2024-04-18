using Microsoft.AspNetCore.Identity;

namespace Library_APIs.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public Guid BookId {  get; set; }
        public List<Book> Book { get; set; }
    }
}
