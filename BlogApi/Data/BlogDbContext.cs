using BlogApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Data
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) 
        {
        
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
