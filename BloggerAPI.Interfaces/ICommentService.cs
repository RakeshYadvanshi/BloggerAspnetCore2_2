using BloggerAPI.DTO.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAPI.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<Comment>> GetComments();
        Task<IEnumerable<Comment>> GetCommentById();
        Task<IEnumerable<Comment>> GetCommentsByPostId();
        Task<IEnumerable<Comment>> GetCommentsByUserId();
        Task<Comment> Add(Comment comment);

        Task<Comment> Update(Comment comment);
        Task<bool> Delete(Comment comment);
    }
}
