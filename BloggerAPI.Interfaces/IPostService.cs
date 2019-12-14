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
        IEnumerable<Post> GetPosts();
        IEnumerable<Post> GetPostById();
        IEnumerable<Post> GetPostsByUserId();
        Post Add(Post Post);
        Post Update(Post Post);
        bool Delete(Post Post);
    }
}
