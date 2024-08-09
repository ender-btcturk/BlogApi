using BlogApi.Data.Entities;
using BlogApi.V1.Requests.Comment;

namespace BlogApi.Interfaces
{
    public interface ICommentRepository
    {
        public Task<List<Comment>> GetAllCommentsAsync(string sort);
        public Task<Comment> CreateCommentAsync(Comment comment);
        public Task<Comment?> GetCommentByIdAsync(int id);
        public Task<Comment?> UpdateCommentAsync(UpdateCommentRequest request, int id);
        public Task<Comment?> DeleteCommentAsync(int id);
    }
}
