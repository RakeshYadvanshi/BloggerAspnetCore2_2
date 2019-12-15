using AutoMapper;
using BloggerAPI.Data;
using BloggerAPI.DTO.Entities;
using BloggerAPI.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BloggerAPI.Tests
{

    public class UserServiceShould
    {
        private readonly UserService _userService;
        public UserServiceShould()
        {
            _userService = new UserService(Helper.DbContext, Helper.Mapper);
        }

        [Fact]
        public void Verify_Give_Users_When_GetUsers_Called()
        {
            var users = _userService.GetUsers().Result;
            Assert.IsAssignableFrom<IEnumerable<User>>(users);
        }
        [Fact]
        public void Verify_Give_User_When_GetUser_Called_For_ExistingUser()
        {
            var userMock = new Mock<User>();
            var user = _userService.Add(userMock.Object).Result;

            var result = _userService.GetUserById(user.Id).Result;
            Assert.Same(user, result);

        }
        [Fact]
        public void Verify_Give_Null_When_GetUser_Called_For_NonExistingUser()
        {
            var result = _userService.GetUserById(It.IsAny<int>()).Result;
            Assert.Null(result);
        }
        [Fact]
        public void Verify_Add_NewUser_When_Add_Called_With_NotNullUser()
        {

            var beforeUsersCount = _userService.GetUsers().Result.Count();

            var userMock = new Mock<User>();
            var user = _userService.Add(userMock.Object).Result;
            var afterUsersCount = _userService.GetUsers().Result.Count();


            Assert.True(beforeUsersCount + 1 == afterUsersCount);

            Assert.Equal(user, _userService.GetUserById(user.Id).Result);

        }
        [Fact]
        public void Verify_Throw_ArgumentNullException_When_Add_Called_With_Null()
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                                     async () => { await _userService.Add(null); });

        }



    }
}
