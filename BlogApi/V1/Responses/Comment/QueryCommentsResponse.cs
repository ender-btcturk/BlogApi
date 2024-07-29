namespace BlogApi.V1.Responses.Comment
{
    public class QueryCommentsResponse
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? AuthorId { get; set; }
        public int? PostId { get; set; }
    }
}
