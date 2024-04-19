using System.ComponentModel.DataAnnotations.Schema;

namespace Library_APIs.Models
{
    public class Request
    {
        public Guid Id { get; set; }
        public ApplicationUser User { get; set; }
        public Book Book { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        [ForeignKey(nameof(Book))]
        public Guid BookId { get; set; }
        public BookState BookeState { get; set; }
    }
}
