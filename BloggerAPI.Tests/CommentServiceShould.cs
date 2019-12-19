using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BloggerAPI.DTO.Entities;
using BloggerAPI.DTO.Enum;
using BloggerAPI.Interfaces;
using BloggerAPI.Services;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace BloggerAPI.Tests
{
    public class CommentServiceShould
    {
        private readonly CommentService _commentService;
        private readonly UserService _userService;
        private readonly PostService _postService;
        private readonly User fakeUser;
        private readonly Post fakePost;
        private readonly User fakeCommentCreatorUserPost;

        public CommentServiceShould()
        {
            _userService = new UserService(Helper.DbContext, Helper.Mapper);
            _postService = new PostService(Helper.DbContext, Helper.Mapper);

            _commentService = new CommentService(Helper.DbContext,
                Helper.Mapper, _postService, _userService);

            fakeUser = _userService.Add(new User(string.Empty, string.Empty, string.Empty, string.Empty, DateTime.Now))
                .Result;
            fakeCommentCreatorUserPost = _userService
                .Add(new User(string.Empty, string.Empty, string.Empty, string.Empty, DateTime.Now)).Result;

            fakePost = _postService.Add(new Post(string.Empty, string.Empty, string.Empty, fakeUser.Id, DateTime.Now))
                .Result;
        }

        #region GetComments

        [Fact]
        public void Verify_Give_Comments_When_GetComments_Called()
        {
            var comments = _commentService.GetComments().Result;
            Assert.IsAssignableFrom<IEnumerable<Comment>>(comments);
        }

        [Fact]
        public void Verify_Give_Comments_When_GetCommentsByPostId_Called()
        {
            var comments = _commentService
                .GetCommentsByPostId(It.IsAny<int>())
                .Result;
            Assert.IsAssignableFrom<IEnumerable<Comment>>(comments);
        }

        [Fact]
        public void Verify_Give_Comments_When_GetCommentsByUserId_Called()
        {
            var comments = _commentService.GetCommentsByUserId(It.IsAny<int>()).Result;
            Assert.IsAssignableFrom<IEnumerable<Comment>>(comments);
        }

        #endregion

        #region CanEdit

        [Theory]
        [InlineData(2, 2, true)] // author is editor 
        [InlineData(3, 2, false)] // author is not editor
        public void Verify_When_CanEdit_Called(int editorId, int authorId, bool expectedResult)
        {
            var user = new User
            {
                Id = editorId
            };
            var comment = new Comment
            {
                CreatedBy = authorId
            };

            Assert.Equal(_commentService.CanEdit(comment, user), expectedResult);
        }


        [Theory]
        [InlineData(CommentOnType.Posts)]
        [InlineData(CommentOnType.Users)]
        public void Verify_Give_Comment_When_GetCommentById_Called_For_ExistingComment(
            CommentOnType commentOnType)
        {
            var fakeComment = new Comment(string.Empty,
                commentOnType == CommentOnType.Users ? fakeUser.Id : fakePost.Id,
                commentOnType.ToString(), DateTime.Now, DateTime.Today, fakeCommentCreatorUserPost.Id);

            var commntAdded = _commentService.Add(commentOnType, fakeComment).Result;

            var result = _commentService.GetCommentById(commntAdded.Id).Result;
            Assert.Same(commntAdded, result);
        }

        [Fact]
        public void Verify_Give_Null_When_GetCommentById_Called_For_NonExistingComment()
        {
            var result = _commentService.GetCommentById(It.IsAny<int>()).Result;
            Assert.Null(result);
        }

        #endregion


        //#region add

        [Theory]
        [InlineData(CommentOnType.Posts)]
        [InlineData(CommentOnType.Users)]
        public void Verify_Add_NewComment_When_Add_Called_With_NotNullUser(CommentOnType commentOnType)
        {
            var beforeCommentsCount = _commentService.GetComments().Result.Count();

            var fakeComment = new Comment(string.Empty,
                commentOnType == CommentOnType.Users ? fakeUser.Id : fakePost.Id,
                commentOnType.ToString(), DateTime.Now, DateTime.Today, fakeCommentCreatorUserPost.Id);

            var post = _commentService.Add(commentOnType, fakeComment).Result;
            var afterCommentsCount = _commentService.GetComments().Result.Count();


            Assert.True(beforeCommentsCount + 1 == afterCommentsCount);

            Assert.Equal(post, _commentService.GetCommentById(fakeComment.Id).Result);
        }

        [Theory]
        [InlineData(CommentOnType.Posts)]
        [InlineData(CommentOnType.Users)]
        public void Verify_Throw_ArgumentNullException_When_Add_Called_With_Null(CommentOnType commentOnType)
        {
          var exception = Assert.ThrowsAsync<ArgumentNullException>(nameof(Comment),
                     async () =>
                    {
                        await _commentService.Add(commentOnType, null);
                    });


            Assert.Contains(nameof(Comment),exception.Result.Message);
        }

        //#endregion


        //#region Update comment
        [Theory]
        [InlineData(CommentOnType.Posts)]
        [InlineData(CommentOnType.Users)]
        public void Verify_Throw_ArgumentNullException_When_Update_Called_With_Null(CommentOnType commentOnType)
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                async () => { await _commentService.Update(commentOnType, null); });
        }

        [Theory]
        [InlineData(CommentOnType.Posts)]
        [InlineData(CommentOnType.Users)]
        public void Verify_Throw_NotSupportedException_When_Update_Called_With_NonExistingEntity(
            CommentOnType commentOnType)
        {
            var fakeComment = new Comment(string.Empty, 10,
                commentOnType.ToString(), DateTime.Now, DateTime.Today, fakeCommentCreatorUserPost.Id);

            Assert.ThrowsAsync<NotSupportedException>(
                async () => { await _commentService.Update(commentOnType, fakeComment); });
        }

        [Theory]
        [InlineData(CommentOnType.Posts)]
        [InlineData(CommentOnType.Users)]
        public void Verify_Comment_Get_Update_When_Update_Called_With_ExistingComment(CommentOnType commentOnType)
        {
            var fakeComment = new Comment(string.Empty,
                commentOnType == CommentOnType.Users ? fakeUser.Id : fakePost.Id,
                commentOnType.ToString(), DateTime.Now, DateTime.Today, fakeCommentCreatorUserPost.Id);
            var addedComment = _commentService.Add(commentOnType, fakeComment).Result;
            fakeComment.CommentText = "comment is modified";
            var updatedComment = _commentService.Update(commentOnType, fakeComment).Result;

            Assert.Equal(updatedComment.CommentText, fakeComment.CommentText);
            Assert.Equal(updatedComment, fakeComment);
        }

        //#endregion


        //#region delete Comment
        [Fact]
        public void Verify_Throw_ArgumentNullException_When_Delete_Called_With_Null()
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                async () => { await _commentService.Delete(null); });
        }

        [Fact]
        public void Verify_Throw_NotSupportedException_When_Delete_Called_With_NonExistingComment()
        {
            Assert.ThrowsAsync<NotSupportedException>(
                async () =>
                {
                    await _commentService.Delete(new Comment(String.Empty, Int32.MaxValue, String.Empty, DateTime.Now,
                        DateTime.MaxValue, fakeCommentCreatorUserPost.Id));
                });
        }

        [Theory]
        [InlineData(CommentOnType.Posts)]
        [InlineData(CommentOnType.Users)]
        public void Verify_Comment_Get_Deleted_When_Get_Called_With_ExistingComment(CommentOnType commentOnType)
        {
            var fakeComment = new Comment(string.Empty,
                commentOnType == CommentOnType.Users ? fakeUser.Id : fakePost.Id,
                commentOnType.ToString(), DateTime.Now, DateTime.Today, fakeCommentCreatorUserPost.Id);
            var addedComment = _commentService.Add(commentOnType, fakeComment).Result;

            var deleteStatus = _commentService.Delete(addedComment).Result;

            Assert.True(deleteStatus);
            Assert.Null(_commentService.GetCommentById(addedComment.Id).Result);
        }


        //#endregion
    }
}
