using BloggerAPI.DTO.Entities;
using BloggerAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BloggerAPI.Data
{
   
    public class BloggerDbContext : DbContext, IBloggerDbContext
    {
        public BloggerDbContext(DbContextOptions<BloggerDbContext> options)
            : base(options)
        {
        }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
      
    }
}
