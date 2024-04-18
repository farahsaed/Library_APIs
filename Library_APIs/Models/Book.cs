using System.ComponentModel.DataAnnotations.Schema;

namespace Library_APIs.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Category Category { get; set; }

        [ForeignKey(nameof(Category))]
        public Guid CategoryId { get; set; }
        public string Image { get; set; }
        public BookState BookState { get; set; }
        public ApplicationUser User { get; set; }

    }
}
