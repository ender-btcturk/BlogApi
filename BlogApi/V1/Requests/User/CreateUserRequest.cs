using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BlogApi.V1.Requests.User
{
    public class CreateUserRequest
    {
        [Required]
        [MaxLength(15, ErrorMessage = "Name cannot be over 15 characters")]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(10, ErrorMessage = "Surname cannot be over 10 characters")]
        public string Surname { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")]
        public string Email { get; set; } = string.Empty;
    }
}
