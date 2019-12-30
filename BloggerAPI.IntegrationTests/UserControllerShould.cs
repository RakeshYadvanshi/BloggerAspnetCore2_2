using System.Collections;
using System.Collections.Generic;
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
    public class UserControllerShould
    {
        private IUserService userService;
        private Mock<IUserService> userServiceMock;
        private UsersController sut;
        public UserControllerShould()
        {
            userServiceMock = new Mock<IUserService>();//new UserService(Helper.DbContext, Helper.Mapper);
            userService = userServiceMock.Object;
            sut = new UsersController(userService, Helper.Mapper);
        }

        [Fact]
        public void Verify_Give_UserViewModel_When_Get_Called()
        {
            userServiceMock.Setup(_ => _.GetUsers()).ReturnsAsync(new List<User>());

            var result = sut.Get().Result;

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<IEnumerable<UserViewModel>>(okResult.Value);


        }



        [Fact]
        public void Verify_Give_UserViewModel_When_Post_Called()
        {
            userServiceMock.Setup(_ => _.Add(It.IsAny<User>()))
                .ReturnsAsync(new Mock<User>().Object);

            var result = sut.Get().Result;

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<UserViewModel>(okResult.Value);


        }


        [Fact]
        public void Verify_Give_Status500InternalServerError_When_Post_Called()
        {
            userServiceMock.Setup(_ => _.Add(It.IsAny<User>()))
                .ReturnsAsync(() => null);

            var result = sut.Get().Result;

            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.IsAssignableFrom<IEnumerable<UserViewModel>>(okResult.Value);


        }
    }
}
