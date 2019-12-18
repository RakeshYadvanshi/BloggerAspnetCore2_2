using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BloggerAPI.DTO.Entities;
using Microsoft.EntityFrameworkCore;

namespace BloggerAPI.Interfaces
{
    public interface IBloggerDbContext
    {
        DbSet<Comment> Comments { get; set; }
        DbSet<Post> Posts { get; set; }
        DbSet<User> Users { get; set; }
        int SaveChanges();
        int SaveChanges(bool acceptAllChangesOnSuccess);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken=default);
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken);
        void Dispose();
    }

}
