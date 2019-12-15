using BloggerAPI.DTO.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAPI.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetPosts();
        Task<Post> GetPostById(int postId);
        Task<IEnumerable<Post>> GetPostsByUserId(int userId);
        Task<Post> Add(Post post);
        Task<Post> Update(Post post);
        Task<bool> Delete(Post post);
        bool CanEdit(Post post, User user);
    }
}
