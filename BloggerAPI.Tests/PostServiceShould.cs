using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BloggerAPI.DTO.Entities;
using BloggerAPI.Interfaces;
using BloggerAPI.Services;
using Moq;
using Xunit;
using Xunit.Abstractions;
namespace BloggerAPI.Tests
{
    public class PostServiceShould
    {
        private PostService _postService;
        private ITestOutputHelper _outputHelper;
        private IUserService _userService;
        private User fakeUser;
        public PostServiceShould(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            _postService = new PostService(Helper.DbContext, Helper.Mapper);
            _userService = new UserService(Helper.DbContext, Helper.Mapper);
            fakeUser = _userService.Add(new User(String.Empty, String.Empty, String.Empty, String.Empty, DateTime.Now)).Result;
        }

        #region GetPosts
        [Fact]
        public void Verify_Give_Posts_When_GetPosts_Called()
        {
            var posts = _postService.GetPosts().Result;
            Assert.IsAssignableFrom<IEnumerable<Post>>(posts);
        }


        #endregion

        #region GetPosts by UserId
        [Fact]
        public void Verify_Give_Posts_When_GetPostsByUserId_Called()
        {
            var posts = _postService.GetPostsByUserId(fakeUser.Id).Result;
            Assert.IsAssignableFrom<IEnumerable<Post>>(posts);
        }


        #endregion

        #region GetUser

        [Fact]
        public void Verify_Give_Post_When_GetPostById_Called_For_ExistingPost()
        {
            //arrange

            var fakePost = new Post(String.Empty, String.Empty, String.Empty, fakeUser.Id, DateTime.Now);
            var postAdded = _postService.Add(fakePost).Result;

            //act
            var result = _postService.GetPostById(postAdded.Id).Result;

            //assert
            Assert.Same(postAdded, result);

        }
        [Fact]
        public void Verify_Give_Null_When_GetPostById_Called_For_NonExistingPost()
        {
            var result = _postService.GetPostById(It.IsAny<int>()).Result;
            Assert.Null(result);
        }

        #endregion

        #region add

        [Fact]
        public void Verify_Add_NewPost_When_Add_Called_With_NotNullUser()
        {

            var beforePostsCount = _postService.GetPosts().Result.Count();

            var fakePost = new Post(String.Empty, String.Empty, String.Empty, fakeUser.Id, DateTime.Now);
            var post = _postService.Add(fakePost).Result;


            var afterPostsCount = _postService.GetPosts().Result.Count();
            Assert.True(beforePostsCount + 1 == afterPostsCount);

            Assert.Equal(post, _postService.GetPostById(fakePost.Id).Result);

        }
        [Fact]
        public void Verify_Throw_ArgumentNullException_When_Add_Called_With_Null()
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                async () => { await _postService.Add(null); });

        }

        #endregion

        #region CanEdit
        [Theory]
        [InlineData(2, 2, true)]// author is editor 
        [InlineData(3, 2, false)] // author is not editor
        public void Verify_When_CanEdit_Called(int editorId, int authorId, bool expectedResult)
        {
            var user = new User
            {
                Id = editorId

            };
            var post = new Post
            {
                CreatedBy = authorId

            };

            Assert.Equal(_postService.CanEdit(post, user), expectedResult);


        }


        #endregion

        #region Update Post
        [Fact]
        public void Verify_Throw_ArgumentNullException_When_Update_Called_With_Null()
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                async () => { await _postService.Update(null); });

        }
        [Fact]
        public void Verify_Throw_NotSupportedException_When_Update_Called_With_NonExistingPost()
        {
            var fakePost = new Post(String.Empty, String.Empty, String.Empty, fakeUser.Id, DateTime.Now); ;
            fakePost.Id = 20;
            var exception = Assert.ThrowsAsync<NotSupportedException>(
                async () => await _postService.Update(fakePost));

            Assert.Contains("exists in our system", exception.Result.Message);


        }

        [Fact]
        public void Verify_Give_Post_When_Update_Called_With_ExistingPost()
        {
            var fakePost = new Post(String.Empty, String.Empty, String.Empty, fakeUser.Id, DateTime.Now); ;
            var post = _postService.Add(fakePost).Result;
            _outputHelper.WriteLine(post.ToString());
            post.PostTitle = "post is modified";

            var updatedPost = _postService.Update(post).Result;
            _outputHelper.WriteLine(updatedPost.ToString());

            Assert.Equal(updatedPost.PostTitle, post.PostTitle);
            Assert.Equal(updatedPost, post);
        }


        //[Fact]
        //public void Verify_Throw_NotSupportedException_When_Update_Called_With_ExistingUser_And_Data_not_Saved()
        //{
        //    var dbContextMock = new Mock<IBloggerDbContext>();

        //    dbContextMock.Setup(x
        //                    => x.SaveChangesAsync(default)).ReturnsAsync(0);


        //    Assert.ThrowsAsync<NotSupportedException>(
        //        async () =>
        //        {
        //            var postMock = new Mock<Post>();
        //            var user = _postService.Add(postMock.Object).Result;
        //            user.PostTitle = "post is modified";
        //            await _postService.Update(user);

        //        });

        //}

        #endregion



        #region delete Post
        [Fact]
        public void Verify_Throw_ArgumentNullException_When_Delete_Called_With_Null()
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                async () => { await _postService.Delete(null); });

        }
        [Fact]
        public void Verify_Throw_NotSupportedException_When_Delete_Called_With_NonExistingUser()
        {
            var fakePost = new Post(String.Empty, String.Empty, String.Empty, fakeUser.Id, DateTime.Now); ;
            fakePost.Id = 20;
            var exception = Assert.ThrowsAsync<NotSupportedException>(
                async () => await _postService.Delete(fakePost));

            Assert.Contains("exists in our system", exception.Result.Message);
        }

        [Fact]
        public void Verify_Post_Get_Deleted_When_Get_Called_With_ExistingPost()
        {

            var fakePost = new Post(String.Empty, String.Empty, String.Empty, fakeUser.Id, DateTime.Now);
            var postAdded = _postService.Add(fakePost).Result;
            _outputHelper.WriteLine(postAdded.ToString());
            var crossVerifyPost = _postService.GetPostById(postAdded.Id).Result;


            var deleteStatus = _postService.Delete(crossVerifyPost).Result;


            Assert.True(deleteStatus);
            Assert.Null(_postService.GetPostById(postAdded.Id).Result);
        }


        #endregion

    }
}
