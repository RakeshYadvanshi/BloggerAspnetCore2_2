using AutoMapper;
using BloggerAPI.Data;
using BloggerAPI.DTO.Entities;
using BloggerAPI.DTO.Enum;
using BloggerAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloggerAPI.Services
{
    public class CommentService : ICommentService, IDisposable
    {
        private readonly IBloggerDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IPostService _postService;
        private readonly IUserService _userService;
        public CommentService(IBloggerDbContext dbContext, IMapper mapper,
                IPostService postService, IUserService userService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _postService = postService;
            _userService = userService;
        }
        public async Task<Comment> Add(CommentOnType commentOn, Comment comment)
        {
            if (!await IsCreatedByExistsByType(comment.CommentOnId, commentOn))
            {
                throw new InvalidOperationException(
                    $"{commentOn.ToString()} does not exists in our system!!");
            }

            if (comment != null)
            {
                _dbContext.Comments.Add(comment);
                await _dbContext.SaveChangesAsync();
                return comment;
            }
            else
            {
                throw new ArgumentNullException(nameof(comment));
            }


        }

        public bool CanEdit(Comment comment, User user)
        {
            return user.Id == comment.CreatedBy;
        }

        public async Task<bool> Delete(Comment comment)
        {

            if (comment != null)
            {
                var dbPost = await _dbContext.Comments.Where(x => x.Id == comment.Id)
                             .FirstOrDefaultAsync();
                if (dbPost != null)
                {
                    _dbContext.Comments.Remove(dbPost);
                    if (await _dbContext.SaveChangesAsync() > 0)
                    {
                        return true;
                    }
                }
                else
                {
                    throw new NotSupportedException("comment does not exists in our system");

                }
            }
            else
            {
                throw new ArgumentNullException(nameof(comment));
            }
            return false;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<Comment> GetCommentById(int commentId)
        {
            return await _dbContext.Comments.Where(x => x.Id == commentId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Comment>> GetComments()
        {
            return await _dbContext.Comments.ToListAsync();

        }

        public async Task<IEnumerable<Comment>> GetCommentsByPostId(int postId)
        {
            return await GetCommetsByType(postId, CommentOnType.Posts);
        }

        private async Task<IEnumerable<Comment>> GetCommetsByType(int postId, CommentOnType type)
        {
            return await _dbContext.Comments.Where(_ => _.CommentOn == type.ToString() && _.CommentOnId == postId).ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetCommentsByUserId(int userId)
        {
            return await GetCommetsByType(userId, CommentOnType.Users);
        }

        public async Task<Comment> Update(CommentOnType commentOn, Comment comment)
        {
            if (comment != null)
            {

                if (!await IsCreatedByExistsByType(comment.CommentOnId, commentOn))
                {
                    throw new InvalidOperationException(
                        $"{commentOn.ToString()} does not exists in our system!!");
                }


                var dbComment = await _dbContext.Comments.Where(x => x.Id == comment.Id)
                                      .FirstOrDefaultAsync();
                if (dbComment != null)
                {
                    _mapper.Map(comment, dbComment);
                    if (await _dbContext.SaveChangesAsync() > 0)
                    {
                        return dbComment;
                    }
                    else
                    {
                        throw new Exception("Unexpected Exception !! concern developer");
                    }
                }
                else
                {
                    throw new NotSupportedException("user does not exists in our system");

                }
            }
            else
            {
                throw new ArgumentNullException(nameof(comment));
            }

        }


        private async Task<bool> IsCreatedByExistsByType(int Id, CommentOnType commentOn)
        {

            switch (commentOn)
            {
                case CommentOnType.Posts:
                    var post = await _postService.GetPostById(Id);
                    return post != null;
                case CommentOnType.Users:
                    var user = await _userService.GetUserById(Id);
                    return user != null;
                default:
                    throw new NotImplementedException(
                        $"{commentOn.ToString()}  is not implemented");
            }
        }
    }
}
