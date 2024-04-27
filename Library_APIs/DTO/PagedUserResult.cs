using Library_APIs.Models;

namespace Library_APIs.DTO
{
    public class PagedUserResult
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }

    }
}
