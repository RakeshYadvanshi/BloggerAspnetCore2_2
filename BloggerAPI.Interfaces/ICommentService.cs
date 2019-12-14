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
        IEnumerable<Comment> GetComments();
        IEnumerable<Comment> GetCommentById();
        IEnumerable<Comment> GetCommentsByPostId();
        IEnumerable<Comment> GetCommentsByUserId();
        Comment Add(Comment comment);

        Comment Update(Comment comment);
        bool Delete(Comment comment);
    }
}
