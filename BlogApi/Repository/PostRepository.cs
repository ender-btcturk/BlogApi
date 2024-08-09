using BlogApi.Data;
using BlogApi.Data.Entities;
using BlogApi.Interfaces;
using BlogApi.V1.Requests.Post;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly BlogDbContext _context;
        public PostRepository(BlogDbContext context)
        {
            _context = context;
        }
        public async Task<Post> CreatePostAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Post?> DeletePostAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return null;
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<List<Post>> GetAllPostsAsync(string sort)
        {
            switch (sort)
            {
                case "desc":
                    return await _context.Posts.OrderByDescending(p => p.CreatedAt).ToListAsync();
                    break;
                case "asc":
                    return await _context.Posts.OrderBy(p => p.CreatedAt).ToListAsync();
                    break;
                default:
                    return await _context.Posts.ToListAsync();
                    break;
            }
        }

        public async Task<Post?> GetPostByIdAsync(int id)
        {
            return await _context.Posts.FindAsync(id);
        }

        public async Task<Post?> UpdatePostAsync(UpdatePostRequest request, int id)
        {
            Post? existingPost = await _context.Posts.FindAsync(id);
            if (existingPost == null)
            {
                return null;
            }

            _context.Posts
                    .Entry(existingPost)
                    .CurrentValues
                    .SetValues(request);
            await _context.SaveChangesAsync();

            return existingPost;
        }
    }
}
