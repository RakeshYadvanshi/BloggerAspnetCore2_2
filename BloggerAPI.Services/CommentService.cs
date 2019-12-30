using AutoMapper;
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
    public class CommentService : ICommentService
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

            if (comment == null) throw new ArgumentNullException(nameof(Comment));

            if (!await IsCreatedByExistsByType(comment.CommentOnId, commentOn))
            {
                throw new NotSupportedException(
                    $"{commentOn.ToString()} does not exists in our system!!");
            }

            _dbContext.Comments.Add(comment);
            await _dbContext.SaveChangesAsync();
            return comment;
        }

        public bool CanEdit(Comment comment, User user)
        {
            return user.Id == comment.CreatedBy;
        }

        public async Task<bool> Delete(Comment comment)
        {

            if (comment != null)
            {
                var dbPost = await _dbContext.Comments
                    .Where(x => x.Id == comment.Id)
                    .FirstOrDefaultAsync();

                if (dbPost != null)
                {
                    _dbContext.Comments.Remove(dbPost);
                    await _dbContext.SaveChangesAsync();
                    return true;
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
        }


        public async Task<Comment> GetCommentById(int commentId)
        {
            return await _dbContext.Comments
                .Where(x => x.Id == commentId)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Comment>> GetComments()
        {
            return await _dbContext.Comments
                .AsNoTracking()
                .ToListAsync();

        }

        public async Task<IEnumerable<Comment>> GetCommentsByPostId(int postId)
        {
            return await GetCommentsByType(postId, CommentOnType.Posts);
        }

        private async Task<IEnumerable<Comment>> GetCommentsByType(int postId, CommentOnType type)
        {
            return await _dbContext.Comments
                .Where(_ => _.CommentOn == type.ToString() && _.CommentOnId == postId)
                .AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetCommentsByUserId(int userId)
        {
            return await GetCommentsByType(userId, CommentOnType.Users);
        }

        public async Task<Comment> Update(CommentOnType commentOn, Comment comment)
        {
            if (comment != null)
            {

                if (!await IsCreatedByExistsByType(comment.CommentOnId, commentOn))
                {
                    throw new NotSupportedException(
                        $"{commentOn.ToString()} does not exists in our system!!");
                }


                var dbComment = await _dbContext.Comments.Where(x => x.Id == comment.Id)
                                      .FirstOrDefaultAsync();
                if (dbComment != null)
                {
                    _mapper.Map(comment, dbComment);
                    await _dbContext.SaveChangesAsync();
                    return dbComment;

                }
                throw new NotSupportedException($"{nameof(comment)} does not exists in our system");


            }
            throw new ArgumentNullException(nameof(comment));


        }


        private async Task<bool> IsCreatedByExistsByType(int id, CommentOnType commentOn)
        {

            if (commentOn == CommentOnType.Posts)
            {
                var post = await _postService.GetPostById(id);
                return post != null;
            }
            else
            {
                var user = await _userService.GetUserById(id);
                return user != null;
            }
        }
    }
}
