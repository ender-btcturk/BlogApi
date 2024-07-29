namespace BlogApi.Data.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? AuthorId { get; set; }
        public User? Author { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
