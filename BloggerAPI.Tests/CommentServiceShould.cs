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

        // Fixture for dependency
        public CommentServiceShould(ContainerFixture container)
        {
            _userService = container.GetContainer.GetInstance<IUserService>();
            _postService = container.GetContainer.GetInstance<IPostService>();
            _commentService = container.GetContainer.GetInstance<ICommentService>();
        }

        #region GetComments

        [Fact]
        public void Verify_Give_Comments_When_GetComments_Called()
        {
            //arrange
            Helper.CommentFake.GetUserCommentExistsInDb(_userService
                , _commentService, out var profile, out var creater);

            //act
            var comments = _commentService.GetComments().Result;

            //assert
            Assert.True(comments.Any());
        }

        [Fact]
        public void Verify_Give_Comments_When_GetCommentsByPostId_Called()
        {
            //arrange
            var comment = Helper.CommentFake.GetPostCommentExistsInDb(_userService
            , _postService, _commentService, out var profile, out var creater);

            //act
            var result = _commentService
                .GetCommentsByPostId(profile.Id)
                .Result;

            //assert
            Assert.True(result.Any());
            Assert.True(result.All(x => x.CommentOnId == profile.Id));


        }

        [Fact]
        public void Verify_Give_Comments_When_GetCommentsByUserId_Called()
        {
            //arrange
            var comment = Helper.CommentFake.GetUserCommentExistsInDb(_userService
                , _commentService, out var profile, out var creater);

            //act
            var result = _commentService.GetCommentsByUserId(profile.Id).Result;

            //assert
            Assert.True(result.Any());
            Assert.True(result.All(x => x.CommentOnId == profile.Id));
        }

        #endregion

        #region CanEdit

        [Theory]
        [InlineData(2, 2, true)] // author is editor 
        [InlineData(3, 2, false)] // author is not editor
        public void Verify_When_CanEdit_Called(int editorId, int authorId, bool expectedResult)
        {
            //arrange
            var user = new User
            {
                Id = editorId
            };
            var comment = new Comment
            {
                CreatedBy = authorId
            };

            //act
            var result = _commentService.CanEdit(comment, user);

            //assert
            Assert.Equal(result, expectedResult);
        }


        [Theory]
        [InlineData(CommentOnType.Posts)]
        [InlineData(CommentOnType.Users)]
        public void Verify_Get_Comment_When_GetCommentById_Called_For_ExistingComment(
            CommentOnType commentOnType)
        {

            //arrange
            Comment comment = new Comment();
            switch (commentOnType)
            {
                case CommentOnType.Posts:
                    comment = Helper.CommentFake.GetPostCommentExistsInDb(
                        _userService, _postService, _commentService, out var Post,
                        out var postCommentCreater);
                    break;
                case CommentOnType.Users:
                    comment = Helper.CommentFake.GetUserCommentExistsInDb(
                        _userService, _commentService, out var user,
                        out var userCommentCreater);
                    break;

            }


            //act
            var dbComment = _commentService.GetCommentById(comment.Id).Result;

            //assert
            Assert.NotNull(dbComment);
            Assert.Equal(comment.Id, dbComment.Id);

        }

        [Fact]
        public void Verify_Null_When_GetCommentById_Called_For_NonExistingComment()
        {
            //arrange
            var commentId = 20;

            //act
            var result = _commentService.GetCommentById(commentId).Result;

            //assert
            Assert.Null(result);
        }

        #endregion


        //#region add

        [Theory]
        [InlineData(CommentOnType.Posts)]
        [InlineData(CommentOnType.Users)]
        public void Verify_NewComment_Added_When_Add_Called_With_ExitingUser(
            CommentOnType commentOnType)
        {


            //arrange
            Comment comment = new Comment();
            switch (commentOnType)
            {
                case CommentOnType.Posts:
                    {
                        var creator = Helper.UserFake.GetUserExistsInDb(_userService);
                        var profile = Helper.PostFake.GetPostExistsInDb(creator,
                            _postService);

                        comment = Helper.CommentFake
                            .GetDefaultPostComment(profile.Id, creator.Id);
                    }
                    break;
                case CommentOnType.Users:
                    {
                        var profile = Helper.UserFake.GetUserExistsInDb(_userService);
                        var creator = Helper.UserFake.GetUserExistsInDb(_userService);


                        comment = Helper.CommentFake
                            .GetDefaultPostComment(profile.Id, creator.Id);

                    }
                    break;

            }


            //act
            var result = _commentService.Add(commentOnType, comment).Result;


            //assert
            var dbComment = _commentService.GetCommentById(comment.Id).Result;
            Assert.Equal(result.Id, dbComment.Id);
            Assert.Equal(result.CommentText, dbComment.CommentText);

        }

        [Theory]
        [InlineData(CommentOnType.Posts)]
        [InlineData(CommentOnType.Users)]
        public void Verify_Throw_ArgumentNullException_When_Add_Called_With_Null(
            CommentOnType commentOnType)
        {
            //assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(nameof(Comment),
                async () =>
                {
                    //act
                    await _commentService.Add(commentOnType, null);
                });


            Assert.Contains(nameof(Comment), exception.Result.Message
                ,StringComparison.OrdinalIgnoreCase);
        }


        [Theory]
        [InlineData(CommentOnType.Posts)]
        [InlineData(CommentOnType.Users)]
        public void Verify_Throw_NotSupportedException_When_Add_Called_With_NonExistingEntity(
            CommentOnType commentOnType)
        {
            //arrange
            Comment comment= new Comment();
            var user = Helper.UserFake.GetUserExistsInDb(_userService);
            switch (commentOnType)
            {
                case CommentOnType.Posts:
                    comment = Helper.CommentFake.GetDefaultPostComment(40, user.Id);
                    break;
                case CommentOnType.Users:
                    comment = Helper.CommentFake.GetDefaultUserComment(40, user.Id);
                    break;

            }

            //assert
            var exception = Assert.ThrowsAsync<NotSupportedException>(
                async () =>
                {
                    //act
                    await _commentService.Add(commentOnType, comment);
                });

            Assert.Contains("not exists in our system", exception.Result.Message,
                StringComparison.OrdinalIgnoreCase);

        }

        //#endregion


        //#region Update comment
        [Theory]
        [InlineData(CommentOnType.Posts)]
        [InlineData(CommentOnType.Users)]
        public void Verify_Throw_ArgumentNullException_When_Update_Called_With_Null(CommentOnType commentOnType)
        {
            //assert
            Assert.ThrowsAsync<ArgumentNullException>(
                async () =>
                {
                    //act
                    await _commentService.Update(commentOnType, null);
                });
        }

        [Theory]
        [InlineData(CommentOnType.Posts)]
        [InlineData(CommentOnType.Users)]
        public void Verify_Throw_NotSupportedException_When_Update_Called_With_NonExistingComment(
            CommentOnType commentOnType)
        {
            //arrange
            Comment comment = new Comment();
            switch (commentOnType)
            {
                case CommentOnType.Posts:
                {
                    var creator = Helper.UserFake.GetUserExistsInDb(_userService);
                    var profile = Helper.PostFake.GetPostExistsInDb(creator,
                        _postService);

                    comment = Helper.CommentFake
                        .GetDefaultPostComment(profile.Id, creator.Id);
                }
                    break;
                case CommentOnType.Users:
                {
                    var profile = Helper.UserFake.GetUserExistsInDb(_userService);
                    var creator = Helper.UserFake.GetUserExistsInDb(_userService);


                    comment = Helper.CommentFake
                        .GetDefaultPostComment(profile.Id, creator.Id);

                }
                    break;

            }

            comment.Id = 20;

            //assert
            var exception = Assert.ThrowsAsync<NotSupportedException>(
                async () =>
                {
                    //act
                    await _commentService.Update(commentOnType, comment);
                });

            Assert.Contains("not exists in our system", exception.Result.Message, 
                StringComparison.OrdinalIgnoreCase);

        }

        [Theory]
        [InlineData(CommentOnType.Posts)]
        [InlineData(CommentOnType.Users)]
        public void Verify_Throw_NotSupportedException_When_Update_Called_With_NonExistingEntity(
                  CommentOnType commentOnType)
        {
            //arrange
            Comment comment = new Comment();
            switch (commentOnType)
            {
                case CommentOnType.Posts:
                    comment = Helper.CommentFake.GetPostCommentExistsInDb(
                        _userService, _postService, _commentService, out var Post,
                        out var postCommentCreater);
                    break;
                case CommentOnType.Users:
                    comment = Helper.CommentFake.GetUserCommentExistsInDb(
                        _userService, _commentService, out var user,
                        out var userCommentCreater);
                    break;

            }

            comment.CommentOnId = 30;

            //assert
            var exception = Assert.ThrowsAsync<NotSupportedException>(
                async () =>
                {
                    //act
                    await _commentService.Update(commentOnType, comment);
                });

            Assert.Contains("not exists in our system", exception.Result.Message
                ,StringComparison.OrdinalIgnoreCase);

        }

        [Theory]
        [InlineData(CommentOnType.Posts)]
        [InlineData(CommentOnType.Users)]
        public void Verify_Comment_GetUpdated_When_Update_Called(
            CommentOnType commentOnType)
        {
            //arrange
            Comment comment = new Comment();
            switch (commentOnType)
            {
                case CommentOnType.Posts:
                    comment = Helper.CommentFake.GetPostCommentExistsInDb(
                        _userService, _postService, _commentService, out var Post,
                        out var postCommentCreater);
                    break;
                case CommentOnType.Users:
                    comment = Helper.CommentFake.GetUserCommentExistsInDb(
                        _userService, _commentService, out var user,
                        out var userCommentCreater);
                    break;

            }

            //act
            comment.CommentText = "comment is modified";
            var updatedComment = _commentService.Update(commentOnType, comment)
                                .Result;

            //assert
            Assert.Equal(updatedComment.CommentText, comment.CommentText);
            Assert.Equal(updatedComment, comment);

        }


        //#endregion


        //#region delete Comment
        [Fact]
        public void Verify_Throw_ArgumentNullException_When_Delete_Called_With_Null()
        {
            //assert
            Assert.ThrowsAsync<ArgumentNullException>(
                async () =>
                {
                    //act
                    await _commentService.Delete(null);
                });
        }

        [Fact]
        public void Verify_Throw_NotSupportedException_When_Delete_Called_With_NonExistingComment()
        {
            //arrange
            var user = Helper.UserFake.GetUserExistsInDb(_userService);
            var comment = new Comment()
            {
                Id = 20,
                CommentOnId = 20,
                CreatedBy = user.Id
            };


            //assert
            Assert.ThrowsAsync<NotSupportedException>(
                async () =>
                {
                    //act
                    await _commentService.Delete(comment);
                });
        }

        [Theory]
        [InlineData(CommentOnType.Posts)]
        [InlineData(CommentOnType.Users)]
        public void Verify_Comment_Get_Deleted_When_Get_Called_With_ExistingComment(CommentOnType commentOnType)
        {
            //arrange
            //arrange
            Comment comment = new Comment();
            switch (commentOnType)
            {
                case CommentOnType.Posts:
                    comment = Helper.CommentFake.GetPostCommentExistsInDb(
                        _userService, _postService, _commentService, out var Post,
                        out var postCommentCreater);
                    break;
                case CommentOnType.Users:
                    comment = Helper.CommentFake.GetUserCommentExistsInDb(
                        _userService, _commentService, out var user,
                        out var userCommentCreater);
                    break;

            }

            //act
            var deleteStatus = _commentService.Delete(comment).Result;

            //assert
            Assert.True(deleteStatus);
            Assert.Null(_commentService.GetCommentById(comment.Id).Result);
        }


        //#endregion

    }
}