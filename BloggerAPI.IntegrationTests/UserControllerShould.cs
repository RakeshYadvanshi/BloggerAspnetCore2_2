using System;
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
    public class UserControllerShould : IClassFixture<ContainerFixture>
    {
        private IUserService userService;
        private Mock<IUserService> userServiceMock;
        private UsersController sut;
        public UserControllerShould(ContainerFixture containerFixture)
        {

            userServiceMock = containerFixture.GetContainer
                .GetInstance<Mock<IUserService>>();
            userService = userServiceMock.Object;
            sut = new UsersController(userService, Helper.Mapper);
        }

        [Fact]
        public void Verify_Give_UserViewModel_When_Get_Called()
        {
            //arrange
            userServiceMock.Setup(_ => _.GetUsers()).ReturnsAsync(new List<User>());

            //act
            var result = sut.Get().Result;

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<IEnumerable<UserViewModel>>(okResult.Value);


        }



        [Fact]
        public void Verify_Give_UserViewModel_When_Post_Called_User_Saved()
        {
            //arrange
            userServiceMock.Setup(_ => _.Add(It.IsAny<User>()))
                .ReturnsAsync(new Mock<User>().Object);
            var UserVM = getDefaultUserViewModel();


            //act
            var result = sut.Post(UserVM).Result;

            //assert
            var okResult = Assert.IsType<CreatedResult>(result);
            Assert.IsAssignableFrom<UserViewModel>(okResult.Value);


        }


        [Fact]
        public void Verify_Give_BadRequestResult_When_Post_Called_User_Not_Saved()
        {
            //arrange
            userServiceMock.Setup(_ => _.Add(It.IsAny<User>()))
                .ReturnsAsync(() => null);
            var UserVM = getDefaultUserViewModel();

            //act
            var result = sut.Post(UserVM).Result;

            //assert
            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Contains("not saved", okResult.Value.ToString(),
                StringComparison.OrdinalIgnoreCase);

        }



        [Fact]
        public void Verify_Updated_When_Put_Called()
        {
            //arrange
            var user = getDefaultUser();
            userServiceMock.Setup(_ => _.Add(It.IsAny<User>()))
                .ReturnsAsync(user);
            userServiceMock.Setup(_ => _.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(user);

            var UserVM = getDefaultUserViewModel();

            //act
            var result = sut.Put(UserVM).Result;

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public void Verify_Status400_When_Put_Called()
        {
            //arrange
            var user = getDefaultUser();
            userServiceMock.Setup(_ => _.Update(It.IsAny<User>()))
                .ReturnsAsync(user);
            userServiceMock.Setup(_ => _.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(() => null);

            var UserVM = getDefaultUserViewModel();

            //act
            var result = sut.Put(UserVM).Result;

            //assert
            var okResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("not found", okResult.Value.ToString()
                , StringComparison.OrdinalIgnoreCase);
        }


        [Fact]
        public void Verify_Status500_When_Put_Called()
        {
            //arrange
            var user = getDefaultUser();
            userServiceMock.Setup(_ => _.Update(It.IsAny<User>()))
                .ReturnsAsync(user);
            userServiceMock.Setup(_ => _.GetUserById(It.IsAny<int>()))
                .ThrowsAsync(new Exception("this is dummy exception message"));

            var UserVM = getDefaultUserViewModel();

            //act
            var result = sut.Put(UserVM).Result;

            //assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("not saved", okResult.Value.ToString()
                , StringComparison.OrdinalIgnoreCase);


        }


        [Fact]
        public void Verify_Deleted_When_Delete_Called()
        {
            //arrange
            var user = getDefaultUser();
            userServiceMock.Setup(_ => _.Delete(It.IsAny<User>()))
                .ReturnsAsync(true);
            userServiceMock.Setup(_ => _.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(user);

            
            //act
            var result = sut.Delete(user.Id).Result;

            //assert
            var okResult = Assert.IsType<OkResult>(result);

        }

        [Fact]
        public void Verify_Status500_When_Delete_Called()
        {
            //arrange
            var user = getDefaultUser();
            userServiceMock.Setup(_ => _.Delete(It.IsAny<User>()))
                .ReturnsAsync(false);
            userServiceMock.Setup(_ => _.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(user);
            
            //act
            var result = sut.Delete(user.Id).Result;

            //assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("not deleted", okResult.Value.ToString()
                , StringComparison.OrdinalIgnoreCase);


        }


        [Fact]
        public void Verify_Status400_When_Delete_Called()
        {
            //arrange
            var user = getDefaultUser();
            userServiceMock.Setup(_ => _.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(()=>null);
            
            //act
            var result = sut.Delete(user.Id).Result;

            //assert
            var okResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("not found", okResult.Value.ToString()
                , StringComparison.OrdinalIgnoreCase);


        }

        private UserViewModel getDefaultUserViewModel()
        {
            return new UserViewModel()
            {
                Email = "rk@gmail.com",
                FirstName = "first name",
                LastName = "last name",
                Password = "password",
                CreatedDate = DateTime.Now
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
