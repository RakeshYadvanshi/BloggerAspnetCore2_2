using AutoMapper;
using BloggerAPI.Data;
using BloggerAPI.DTO.Entities;
using BloggerAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloggerAPI.Services
{
    public class PostService : IPostService
    {
        private readonly IBloggerDbContext _dbContext;
        private readonly IMapper _mapper;

        public PostService(IBloggerDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;


        }

        public async Task<Post> Add(Post post)
        {
            if (post == null) throw new ArgumentNullException(nameof(post));

            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();
            return post;
        }

        public bool CanEdit(Post post, User user)
        {
            return user.Id == post.CreatedBy;
        }

        public async Task<bool> Delete(Post post)
        {

            if (post != null)
            {
                var dbPost = await _dbContext.Posts.Where(x => x.Id == post.Id)
                             .FirstOrDefaultAsync();
                if (dbPost != null)
                {
                    _dbContext.Posts.Remove(dbPost);
                    if (await _dbContext.SaveChangesAsync() > 0)
                    {
                        return true;
                    }
                }
                else
                {
                    throw new NotSupportedException("user does not exists in our system");

                }
            }
            else
            {
                throw new ArgumentNullException(nameof(post));
            }
            return false;
        }



        public async Task<Post> GetPostById(int postId)
        {
            return await _dbContext.Posts.Where(x => x.Id == postId).FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<Post>> GetPosts()
        {
            return await _dbContext.Posts.ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsByUserId(int userId)
        {
            return await _dbContext.Posts.Where(_ => _.CreatedBy == userId).ToListAsync();

        }

        public async Task<Post> Update(Post post)
        {
            if (post == null) throw new ArgumentNullException(nameof(post));

            var dbUser = await _dbContext.Posts.Where(x => x.Id == post.Id).FirstOrDefaultAsync();
            if (dbUser == null) throw new NotSupportedException("user does not exists in our system");

            _mapper.Map(post, dbUser);
            if (await _dbContext.SaveChangesAsync() > 0)
            {
                return dbUser;
            }

            throw new Exception("Unexpected Exception !! concern developer");

        }
    }
}
