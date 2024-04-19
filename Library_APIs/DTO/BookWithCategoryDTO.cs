using Library_APIs.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library_APIs.DTO
{
    public class BookWithCategoryDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Category Category { get; set; }
        public IFormFile Image { get; set; }
    }
}
