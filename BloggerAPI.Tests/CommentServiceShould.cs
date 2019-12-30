using System;
using System.Collections.Generic;
using System.Linq;
using BloggerAPI.DTO.Entities;
using BloggerAPI.DTO.Enum;
using BloggerAPI.Interfaces;
using Moq;
using Xunit;

namespace BloggerAPI.Tests
{
    public class CommentServiceShould : IClassFixture<ContainerFixture>
    {
        private readonly ICommentService _commentService;
        private readonly IUserService _userService;
        private readonly IPostService _postService;
        private readonly User fakeUser;
        private readonly Post fakePost;
        private readonly User fakeCommentCreatorUserPost;

        // Fixture for dependency
        public CommentServiceShould(ContainerFixture container)
        {
            _userService = container.GetContainer.GetInstance<IUserService>();
            _postService = container.GetContainer.GetInstance<IPostService>();
            _commentService = container.GetContainer.GetInstance<ICommentService>();

            fakeUser = _userService.Add(new User())
                .Result;
            fakeCommentCreatorUserPost = _userService
                .Add(new User()).Result;

            fakePost = _postService.Add(new Post())
                .Result;
        }

        #region GetComments

        [Fact]
        public void Verify_Give_Comments_When_GetComments_Called()
        {
            //Arrange

            //Act

            //Assert

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
            var fakeComment = new Comment()
            {

                CommentOnId = commentOnType == CommentOnType.Users ? fakeUser.Id : fakePost.Id,
                CommentOn = commentOnType.ToString(),
                CreatedBy = fakeCommentCreatorUserPost.Id
            };

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

            var fakeComment = new Comment()
            {

                CommentOnId = commentOnType == CommentOnType.Users ? fakeUser.Id : fakePost.Id,
                CommentOn = commentOnType.ToString(),
                CreatedBy = fakeCommentCreatorUserPost.Id
            };

            var post = _commentService.Add(commentOnType, fakeComment).Result;
            var afterCommentsCount = _commentService.GetComments().Result.Count();


            Assert.NotEqual(beforeCommentsCount, afterCommentsCount);

            Assert.Equal(post, _commentService.GetCommentById(fakeComment.Id).Result);
        }

        [Theory]
        [InlineData(CommentOnType.Posts)]
        [InlineData(CommentOnType.Users)]
        public void Verify_Throw_ArgumentNullException_When_Add_Called_With_Null(CommentOnType commentOnType)
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(nameof(Comment),
                async () => { await _commentService.Add(commentOnType, null); });


            Assert.Contains(nameof(Comment), exception.Result.Message);
        }


        [Theory]
        [InlineData(CommentOnType.Posts)]
        [InlineData(CommentOnType.Users)]
        public void Verify_Throw_NotSupportedException_When_Add_Called_With_NonExistingEntity(
            CommentOnType commentOnType)
        {
            var fakeComment = new Comment()
            {
                CommentOnId = 20,
                CommentOn = commentOnType.ToString(),
                CreatedBy = fakeCommentCreatorUserPost.Id
            };
            fakeComment.Id = 20;
            var exception = Assert.ThrowsAsync<NotSupportedException>(
                async () => await _commentService.Add(commentOnType, fakeComment));

            Assert.Contains("exists in our system", exception.Result.Message);

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
        public void Verify_Throw_NotSupportedException_When_Update_Called_With_NonExistingComment(
            CommentOnType commentOnType)
        {
            var fakeComment = new Comment()
            {

                CommentOnId = commentOnType == CommentOnType.Users ? fakeUser.Id : fakePost.Id,
                CommentOn = commentOnType.ToString(),
                CreatedBy = fakeCommentCreatorUserPost.Id
            };
            fakeComment.Id = 20;
            var exception = Assert.ThrowsAsync<NotSupportedException>(
                async () => await _commentService.Update(commentOnType, fakeComment));

            Assert.Contains("exists in our system", exception.Result.Message, StringComparison.OrdinalIgnoreCase);

        }

        [Theory]
        [InlineData(CommentOnType.Posts)]
        [InlineData(CommentOnType.Users)]
        public void Verify_Throw_NotSupportedException_When_Update_Called_With_NonExistingEntity(
                  CommentOnType commentOnType)
        {
            var fakeComment = new Comment()
            {
                CommentOnId = 20,
                CommentOn = commentOnType.ToString(),
                CreatedBy = fakeCommentCreatorUserPost.Id
            };
            fakeComment.Id = 20;
            var exception = Assert.ThrowsAsync<NotSupportedException>(
                async () => await _commentService.Update(commentOnType, fakeComment));

            Assert.Contains("exists in our system", exception.Result.Message);

        }

        [Theory]
        [InlineData(CommentOnType.Posts)]
        //[InlineData(CommentOnType.Users)]
        public void Verify_Comment_GetUpdated_When_Update_Called(
            CommentOnType commentOnType)
        {
            var fakeComment = new Comment()
            {
                CommentOnId = commentOnType == CommentOnType.Users ? fakeUser.Id : fakePost.Id,
                CommentOn = commentOnType.ToString(),
                CreatedBy = fakeCommentCreatorUserPost.Id
            };

            var comment = _commentService.Add(commentOnType, fakeComment).Result;

            //act
            comment.CommentText = "comment is modified";
            var updatedComment = _commentService.Update(commentOnType, comment).Result;

            //assert
            Assert.Equal(updatedComment.CommentText, comment.CommentText);
            Assert.Equal(updatedComment, comment);

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
                    await _commentService.Delete(new Comment()
                    {
                        CommentOnId = 20,
                        CreatedBy = fakeCommentCreatorUserPost.Id
                    });
                });
        }

        [Theory]
        [InlineData(CommentOnType.Posts)]
        [InlineData(CommentOnType.Users)]
        public void Verify_Comment_Get_Deleted_When_Get_Called_With_ExistingComment(CommentOnType commentOnType)
        {
            var fakeComment = new Comment()
            {

                CommentOnId = commentOnType == CommentOnType.Users ? fakeUser.Id : fakePost.Id,
                CommentOn = commentOnType.ToString(),
                CreatedBy = fakeCommentCreatorUserPost.Id
            };
            var addedComment = _commentService.Add(commentOnType, fakeComment).Result;

            var deleteStatus = _commentService.Delete(addedComment).Result;

            Assert.True(deleteStatus);
            Assert.Null(_commentService.GetCommentById(addedComment.Id).Result);
        }


        //#endregion
    }
}