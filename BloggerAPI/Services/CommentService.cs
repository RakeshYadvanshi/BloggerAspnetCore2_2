using BloggerAPI.Data;
using BloggerAPI.DTO.Entities;
using BloggerAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloggerAPI.Services
{
    public class CommentService : ICommentService, IDisposable
    {
        private BloggerDbContext _dbContext;
        public CommentService(BloggerDbContext dbContext)
        {
            _dbContext = dbContext;

        }
        public async Task<Comment> Add(Comment comment)
        {
            _dbContext.Comments.Add(comment);
            await _dbContext.SaveChangesAsync();
            return comment;
        }

        public Task<bool> Delete(Comment comment)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public Task<IEnumerable<Comment>> GetCommentById()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Comment>> GetComments()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Comment>> GetCommentsByPostId()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Comment>> GetCommentsByUserId()
        {
            throw new NotImplementedException();
        }

        public Task<Comment> Update(Comment comment)
        {
            throw new NotImplementedException();
        }
    }
}
