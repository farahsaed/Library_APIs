using Azure.Identity;
using System.ComponentModel.DataAnnotations;

namespace Library_APIs.DTO
{
    public class UserDataDTO
    {
        //public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; } 
        public string Email { get; set; }
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

    }
}
