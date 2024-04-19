using Library_APIs.Models;

namespace Library_APIs.DTO
{
    internal class PagedBookResult
    {
        public IEnumerable<Book> books { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}