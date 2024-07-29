using System.ComponentModel.DataAnnotations;

namespace BlogApi.V1.Requests.Comment
{
    public class UpdateCommentRequest
    {
        
        [Required]
        [MaxLength(280, ErrorMessage = "Content cannot be over 280 characters")]
        public string Content { get; set; } = string.Empty;
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public int? AuthorId { get; set; }
        [Required]
        public int? PostId { get; set; }
    }
}
