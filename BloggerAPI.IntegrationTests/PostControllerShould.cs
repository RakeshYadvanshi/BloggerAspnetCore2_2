using System;
using System.Collections;
using System.Collections.Generic;
using AutoMapper;
using BloggerAPI.Controllers;
using BloggerAPI.DTO.Entities;
using BloggerAPI.DTO.ViewModels;
using BloggerAPI.Interfaces;
using BloggerAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using Newtonsoft.Json.Serialization;
using StructureMap.Graph;

namespace BloggerAPI.IntegrationTests
{
    public class PostControllerShould : IClassFixture<ContainerFixture>
    {
        private IPostService _postService;
        private Mock<IPostService> _postServiceMock;

        private IUserService _userService;
        private Mock<IUserService> userServiceMock;

        private PostsController sut;
        public PostControllerShould(ContainerFixture containerFixture)
        {

            _postServiceMock = containerFixture.GetContainer
                .GetInstance<Mock<IPostService>>();
            _postService = _postServiceMock.Object;

            userServiceMock = containerFixture.GetContainer
                .GetInstance<Mock<IUserService>>();
            _userService = userServiceMock.Object;

            sut = new PostsController(_postService, containerFixture.GetContainer
                .GetInstance<IMapper>(), _userService);
        }

        [Fact]
        public void Verify_Give_PostViewModel_When_Get_Called()
        {
            //arrange
            _postServiceMock.Setup(_ => _.GetPostsByUserId(It.IsAny<int>()))
                .ReturnsAsync(new List<Post>() { GetDefaultPost() });
            var userId = 10;

            //act
            var result = sut.Get(userId).Result;

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<IEnumerable<PostViewModel>>(okResult.Value);


        }



        [Fact]
        public void Verify_NotFound_When_Post_Called_With_User_Not_In_System()
        {
            //arrange
            userServiceMock.Setup(_ => _.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(() => null);

            var postVM = getDefaultPostViewModel();
            var user = getDefaultUser();

            //act
            var result = sut.Post(user.Id, postVM).Result;

            //assert
            var okResult = Assert.IsType<NotFoundObjectResult>(result);

        }


        [Fact]
        public void Verify_Give_BadRequestResult_When_Post_Called_Post_Not_Saved()
        {
            //arrange
            var user = getDefaultUser();
            var postVM = getDefaultPostViewModel();

            _postServiceMock.Setup(_ => _.Add(It.IsAny<Post>()))
                .ReturnsAsync(() => null);
            userServiceMock.Setup(_ => _.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(user);


            //act
            var result = sut.Post(user.Id, postVM).Result;

            //assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("not saved", okResult.Value.ToString(),
                StringComparison.OrdinalIgnoreCase);

        }



        [Fact]
        public void Verify_Updated_When_Post_Called()
        {
            //arrange
            var user = getDefaultUser();
            var post = GetDefaultPost();

            var postVM = getDefaultPostViewModel();

            _postServiceMock.Setup(_ => _.Add(It.IsAny<Post>()))
                .ReturnsAsync(post);
            userServiceMock.Setup(_ => _.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(user);


            //act
            var result = sut.Post(user.Id, postVM).Result;

            //assert
            var okResult = Assert.IsType<CreatedResult>(result);
        }


        [Fact]
        public void Verify_NotFound_When_Put_Called_With_User_Not_In_System()
        {
            //arrange
            var user = getDefaultUser();
            var postVM = getDefaultPostViewModel();

            userServiceMock.Setup(_ => _.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(() => null);

            //act
            var result = sut.Put(user.Id, postVM).Result;

            //assert
            var okResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("not found", okResult.Value.ToString()
                , StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Verify_NotFound_When_Put_Called_With_Post_Not_In_System()
        {
            //arrange
            var user = getDefaultUser();
            var postVM = getDefaultPostViewModel();

            _postServiceMock.Setup(_ => _.GetPostById(It.IsAny<int>()))
                .ReturnsAsync(() => null);
            userServiceMock.Setup(_ => _.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(user);

            //act
            var result = sut.Put(user.Id, postVM).Result;

            //assert
            var okResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("not found", okResult.Value.ToString()
                , StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Verify_UnAuthorized_When_Put_Called()
        {
            //arrange
            var user = getDefaultUser();
            var post = GetDefaultPost();
            var postVM = getDefaultPostViewModel();

            _postServiceMock.Setup(_ => _.GetPostById(It.IsAny<int>()))
                .ReturnsAsync(post);
            userServiceMock.Setup(_ => _.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(user);
            _postServiceMock.Setup(_ => _.CanEdit(It.IsAny<Post>(),It.IsAny<User>()))
                .Returns(false);

            //act
            var result = sut.Put(user.Id, postVM).Result;

            //assert
            var okResult = Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public void Verify_Exception_When_Put_Called()
        {
            //arrange
            var user = getDefaultUser();
            var postVM = getDefaultPostViewModel();

            userServiceMock.Setup(_ => _.GetUserById(It.IsAny<int>()))
                .ThrowsAsync(new Exception("this is dummy exception"));
            

            //act
            var result = sut.Put(user.Id, postVM).Result;

            //assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void Verify_OkResult_When_Put_Called()
        {
            //arrange
            var user = getDefaultUser();
            var post = GetDefaultPost();
            var postVM = getDefaultPostViewModel();

            _postServiceMock.Setup(_ => _.GetPostById(It.IsAny<int>()))
                .ReturnsAsync(post);
            userServiceMock.Setup(_ => _.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(user);
            _postServiceMock.Setup(_ => _.CanEdit(It.IsAny<Post>(), It.IsAny<User>()))
                .Returns(true);
            _postServiceMock.Setup(_ => _.Update(It.IsAny<Post>()))
                .ReturnsAsync(post);

            //act
            var result = sut.Put(user.Id, postVM).Result;

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result);


        }


        [Fact]
        public void Verify_NotFound_When_Delete_Called_Post_Not_In_System()
        {
            //arrange
            var user = getDefaultUser();
            var post = GetDefaultPost();

            _postServiceMock.Setup(_ => _.GetPostById(It.IsAny<int>()))
                .ReturnsAsync(()=>null);

            //act
            var result = sut.Delete(user.Id,post.Id).Result;

            //assert
            var okResult = Assert.IsType<NotFoundObjectResult>(result);

        }


        [Fact]
        public void Verify_NotFound_When_Delete_Called_User_Not_In_System()
        {
            //arrange
            var user = getDefaultUser();
            var post = GetDefaultPost();

            _postServiceMock.Setup(_ => _.GetPostById(It.IsAny<int>()))
                .ReturnsAsync(() => post);
            userServiceMock.Setup(_ => _.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(() => null);

            //act
            var result = sut.Delete(user.Id, post.Id).Result;

            //assert
            var okResult = Assert.IsType<NotFoundObjectResult>(result);

        }


        [Fact]
        public void Verify_UnAuthorized_When_Delete_Called_User_CanNot_Delete_Post()
        {
            //arrange
            var user = getDefaultUser();
            var post = GetDefaultPost();

            _postServiceMock.Setup(_ => _.GetPostById(It.IsAny<int>()))
                .ReturnsAsync(() => post);
            userServiceMock.Setup(_ => _.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(() => user);
            _postServiceMock.Setup(_ => _.CanEdit(It.IsAny<Post>(),It.IsAny<User>()))
                .Returns(false);

            //act
            var result = sut.Delete(user.Id, post.Id).Result;

            //assert
            var okResult = Assert.IsType<UnauthorizedObjectResult>(result);

        }

        [Fact]
        public void Verify_Not_Deleted_When_Delete_Called()
        {
            //arrange
            var user = getDefaultUser();
            var post = GetDefaultPost();

            _postServiceMock.Setup(_ => _.GetPostById(It.IsAny<int>()))
                .ReturnsAsync(() => post);
            userServiceMock.Setup(_ => _.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(() => user);
            _postServiceMock.Setup(_ => _.CanEdit(It.IsAny<Post>(), It.IsAny<User>()))
                .Returns(true);
            _postServiceMock.Setup(_ => _.Delete(It.IsAny<Post>()))
                .ReturnsAsync(false);

            //act
            var result = sut.Delete(user.Id, post.Id).Result;

            //assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result);


        }


        [Fact]
        public void Verify_Deleted_When_Delete_Called()
        {
            //arrange
            var user = getDefaultUser();
            var post = GetDefaultPost();

            _postServiceMock.Setup(_ => _.GetPostById(It.IsAny<int>()))
                .ReturnsAsync(() => post);
            userServiceMock.Setup(_ => _.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(() => user);
            _postServiceMock.Setup(_ => _.CanEdit(It.IsAny<Post>(), It.IsAny<User>()))
                .Returns(true);
            _postServiceMock.Setup(_ => _.Delete(It.IsAny<Post>()))
                .ReturnsAsync(true);

            //act
            var result = sut.Delete(user.Id, post.Id).Result;

            //assert
            var okResult = Assert.IsType<OkResult>(result);


        }




        private PostViewModel getDefaultPostViewModel()
        {
            return new PostViewModel()
            {
                CreatedBy = 0,
                CreatedDate = DateTime.Now,
                Description = "description",
                PostTitle = $"post title {DateTime.Now}",
                ShortDescription = $"short description {DateTime.Now}"
            };
        }


        public static Post GetDefaultPost()
        {
            return new Post()
            {
                CreatedBy = 0,
                CreatedDate = DateTime.Now,
                Description = "description",
                PostTitle = $"post title {DateTime.Now}",
                ShortDescription = $"short description {DateTime.Now}"
            };
        }

        private User getDefaultUser()
        {
            return new User()
            {
                Id = 1,
                Email = "rk@gmail.com",
                FirstName = "first name",
                LastName = "last name",
                Password = "password",
                CreatedDate = DateTime.Now
            };
        }
    }
}
