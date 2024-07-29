namespace BlogApi.V1.Responses.Post
{
    public class CreatePostResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? AuthorId { get; set; }
    }
}
