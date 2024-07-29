namespace BlogApi.Data.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? AuthorId { get; set; }
        public User? Author { get; set; }
        public int? PostId { get; set; }
        public Post? Post { get; set; }
    }
}
