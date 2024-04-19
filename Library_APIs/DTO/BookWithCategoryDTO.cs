using Library_APIs.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library_APIs.DTO
{
    public class BookWithCategoryDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public Guid CategoryId { get; set; }
        public IFormFile Image { get; set; }
    }
}
