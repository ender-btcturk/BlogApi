using System.ComponentModel.DataAnnotations;

namespace BlogApi.V1.Requests.Post
{
    public class UpdatePostRequest
    {
        [Required]
        [MaxLength(45, ErrorMessage = "Title cannot be over 45 characters")]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MaxLength(280, ErrorMessage = "Content cannot be over 280 characters")]
        public string Content { get; set; } = string.Empty;
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public int? AuthorId { get; set; }
    }
}
