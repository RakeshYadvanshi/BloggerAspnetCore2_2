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
        Task<Post> Add(Post Post);
        Task<Post> Update(Post Post);
        Task<bool> Delete(Post Post);
    }
}
