using BloggerAPI.DTO.Entities;
using BloggerAPI.DTO.Enum;
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
        Task<Comment> GetCommentById(int commentId);
        Task<IEnumerable<Comment>> GetCommentsByPostId(int postId);
        Task<IEnumerable<Comment>> GetCommentsByUserId(int userId);
        Task<Comment> Add(CommentOnType commentOn, Comment comment);
        Task<Comment> Update(CommentOnType commentOn,Comment comment);
        Task<bool> Delete(Comment comment);
        bool CanEdit(Comment comment, User user);
    }
}
