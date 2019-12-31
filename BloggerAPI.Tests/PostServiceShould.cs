using System;
using System.Collections.Generic;
using System.Linq;
using BloggerAPI.DTO.Entities;
using BloggerAPI.Interfaces;
using Castle.Core;
using Moq;
using Xunit;
using Xunit.Abstractions;
namespace BloggerAPI.Tests
{
    public class PostServiceShould : IClassFixture<ContainerFixture>
    {
        private readonly IPostService _postService;
        private IUserService _userService;
        private readonly ITestOutputHelper _outputHelper;
        public PostServiceShould(ITestOutputHelper outputHelper,
            ContainerFixture container)
        {
            _outputHelper = outputHelper;
            _postService = container.GetContainer.GetInstance<IPostService>();
            _userService = container.GetContainer.GetInstance<IUserService>();
        }

        #region GetPosts
        [Fact]
        public void Verify_Give_Posts_When_GetPosts_Called()
        {
            //arrange
            var post = Helper.PostFake.GetDefaultPost();
            var postAdded = _postService.Add(post);
            //post.Id = postAdded.Id;

            //act
            var posts = _postService.GetPosts().Result;

            //assert
            //Assert.Contains(post, posts);
            Assert.True(posts.Any());


        }
        #endregion

        #region GetPosts by UserId
        [Fact]
        public void Verify_Give_Posts_When_GetPostsByUserId_Called()
        {
            //arrange
            var user = Helper.UserFake.GetUserExistsInDb(_userService);
            var post = Helper.PostFake.GetPostExistsInDb(user, _postService);

            //act
            var posts = _postService.GetPostsByUserId(user.Id).Result;

            //assert
            //Assert.Contains(post, posts,new PostEquality());
            Assert.True(posts.Any());
            Assert.True(posts.First().CreatedBy == post.CreatedBy);
        }


        #endregion

        #region GetUser

        [Fact]
        public void Verify_Give_Post_When_GetPostById_Called_For_ExistingPost()
        {
            //arrange
            var user = Helper.UserFake.GetUserExistsInDb(_userService);
            var post = Helper.PostFake.GetPostExistsInDb(user, _postService);

            //act
            var result = _postService.GetPostById(post.Id).Result;

            //assert
            Assert.Equal(post.Id, result.Id);
            Assert.Equal(post.PostTitle, result.PostTitle);


        }
        [Fact]
        public void Verify_Give_Null_When_GetPostById_Called_For_NonExistingPost()
        {
            //arrange
            var userId = 20;

            //act
            var result = _postService.GetPostById(userId).Result;

            //assert
            Assert.Null(result);
        }

        #endregion

        #region add

        [Fact]
        public void Verify_NewPost_Added_When_Add_Called_With_ExitingUser()
        {
            //arrange
            var user = Helper.UserFake.GetUserExistsInDb(_userService);
            var post = Helper.PostFake.GetPostByUser(user);

            //act
            var postAdded = _postService.Add(post).Result;

            //assert
            var posts = _postService.GetPosts().Result;
            var dbPost = _postService.GetPostById(post.Id).Result;

            Assert.True(posts.Any());
            Assert.NotNull(dbPost);

        }
        [Fact]
        public void Verify_Throw_ArgumentNullException_When_Add_Called_With_Null()
        {
            //assert
            Assert.ThrowsAsync<ArgumentNullException>(
                async () =>
                {
                    //act
                    await _postService.Add(null);
                });

        }

        #endregion

        #region CanEdit
        [Theory]
        [InlineData(2, 2, true)]// author is editor 
        [InlineData(3, 2, false)] // author is not editor
        public void Verify_When_CanEdit_Called(int editorId, int authorId, bool expectedResult)
        {
            //arrange
            var user = new User
            {
                Id = editorId

            };
            var post = new Post
            {
                CreatedBy = authorId

            };

            //act
            bool canEdit = _postService.CanEdit(post, user);

            //asert
            Assert.Equal(canEdit, expectedResult);
        }


        #endregion

        #region Update Post
        [Fact]
        public void Verify_Throw_ArgumentNullException_When_Update_Called_With_Null()
        {
            //assert
            Assert.ThrowsAsync<ArgumentNullException>(
                async () =>
                {
                    //act
                    await _postService.Update(null);
                });

        }
        [Fact]
        public void Verify_Throw_NotSupportedException_When_Update_Called_With_NonExistingPost()
        {
            //arrange
            var user = Helper.UserFake.GetUserExistsInDb(_userService);
            var post = Helper.PostFake.GetPostExistsInDb(user, _postService);

            post.Id = 20;

            //assert
            var exception = Assert.ThrowsAsync<NotSupportedException>(
                async () =>
                {
                    //act
                    await _postService.Update(post);

                });
            Assert.Contains("not exists in our system", exception.Result.Message
                , StringComparison.OrdinalIgnoreCase);


        }

        [Fact]
        public void Verify__Post_Get_Updated_When_Update_Called_With_ExistingPost()
        {

            //arrange
            var user = Helper.UserFake.GetUserExistsInDb(_userService);
            var post = Helper.PostFake.GetPostExistsInDb(user, _postService);

            post.PostTitle = "post is modified";

            //act
            var updatedPost = _postService.Update(post).Result;
            _outputHelper.WriteLine(updatedPost.ToString());

            //assert

            var dbPost = _postService.GetPostById(post.Id).Result;

            Assert.Equal(dbPost.PostTitle, post.PostTitle);
            Assert.Equal(dbPost.Id, post.Id);

            //Assert.Equal(updatedPost, post);
        }


        #endregion



        #region delete Post
        [Fact]
        public void Verify_Throw_ArgumentNullException_When_Delete_Called_With_Null()
        {
            //assert
            Assert.ThrowsAsync<ArgumentNullException>(
                async () =>
                {
                    //act
                    await _postService.Delete(null);
                });

        }

        [Fact]
        public void Verify_Post_Get_Deleted_When_Get_Called_With_ExistingPost()
        {

            //arrange
            var user = Helper.UserFake.GetUserExistsInDb(_userService);
            var post = Helper.PostFake.GetPostExistsInDb(user, _postService);

            //act
            var deleteStatus = _postService.Delete(post).Result;


            //assert
            var dbPost = _postService.GetPostById(post.Id).Result;
            Assert.True(deleteStatus);
            Assert.Null(dbPost);
        }

        #endregion



    }
}
