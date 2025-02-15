using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace EMSystemTask.Models
{
    public class UserInfo
    {
        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? ProfileImage { get; set; }

        [NotMapped]
        public virtual IFormFile imageFile { get; set; } 
    
    }
}
