using BlogApi.Data.Entities;
using BlogApi.V1.Requests.Post;

namespace BlogApi.Interfaces
{
    public interface IPostRepository
    {
        public Task<List<Post>> GetAllPostsAsync(string sort);
        public Task<Post> CreatePostAsync(Post post);
        public Task<Post?> GetPostByIdAsync(int id);
        public Task<Post?> UpdatePostAsync(UpdatePostRequest request, int id);
        public Task<Post?> DeletePostAsync(int id);
    }
}
