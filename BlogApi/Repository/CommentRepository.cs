using BlogApi.Data;
using BlogApi.Data.Entities;
using BlogApi.Interfaces;
using BlogApi.V1.Requests.Comment;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace BlogApi.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly BlogDbContext _context;
        public CommentRepository(BlogDbContext context)
        {
            _context = context;
        }
        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment?> DeleteCommentAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return null;
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<List<Comment>> GetAllCommentsAsync(string sort)
        {
            switch (sort)
            {
                case "desc":
                    return await _context.Comments.OrderByDescending(c => c.CreatedAt).ToListAsync();
                    break;
                case "asc":
                    return await _context.Comments.OrderBy(c => c.CreatedAt).ToListAsync();
                    break;
                default:
                    return await _context.Comments.ToListAsync();
                    break;
            }
        }

        public async Task<Comment?> GetCommentByIdAsync(int id)
        {
            return await _context.Comments.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Comment?> UpdateCommentAsync(UpdateCommentRequest request, int id)
        {
            Comment? existingComment = await _context.Comments.FindAsync(id);
            if (existingComment == null)
            {
                return null;
            }

            _context.Comments
                    .Entry(existingComment)
                    .CurrentValues
                    .SetValues(request);
            await _context.SaveChangesAsync();

            return existingComment;
        }
    }
}
