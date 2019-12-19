using AutoMapper;
using BloggerAPI.Data;
using BloggerAPI.DTO.Entities;
using BloggerAPI.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BloggerAPI.Interfaces;
using Xunit;
using Xunit.Abstractions;
namespace BloggerAPI.Tests
{

    public class UserServiceShould
    {
        private readonly UserService _userService;
        private readonly ITestOutputHelper _outputHelper;
        public UserServiceShould(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            var dbService = Helper.DbContext;
            _userService = new UserService(dbService, Helper.Mapper);

        }

        #region GetUsers
        [Fact]
        public void Verify_Give_Users_When_GetUsers_Called()
        {
            var users = _userService.GetUsers().Result;
            Assert.IsAssignableFrom<IEnumerable<User>>(users);
        }


        #endregion

        #region GetUser

        [Fact]
        public void Verify_Give_User_When_GetUserById_Called_For_ExistingUser()
        {
            var user = new User(String.Empty, String.Empty, String.Empty, String.Empty, DateTime.Now);

            var userAdded = _userService.Add(user).Result;

            var result = _userService.GetUserById(user.Id).Result;

            Assert.NotNull(result);
            Assert.Same( result, userAdded);

        }
        [Fact]
        public void Verify_Give_Null_When_GetUserById_Called_For_NonExistingUser()
        {
            var result = _userService.GetUserById(10).Result;
            Assert.Null(result);
        }

        #endregion

        #region add

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

        #endregion

        #region Update User
        [Fact]
        public void Verify_Throw_ArgumentNullException_When_Update_Called_With_Null()
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                async () => { await _userService.Update(null); });

        }
        [Fact]
        public void Verify_Throw_NotSupportedException_When_Update_Called_With_NonExistingUser()
        {
            var user = new User(String.Empty, String.Empty, String.Empty, String.Empty, DateTime.Now);

            var exception = Assert.ThrowsAsync<NotSupportedException>(
                   async () => await _userService.Update(user));

            Assert.Contains("exists in our system", exception.Result.Message);
        }



        [Fact]
        public void Verify_User_Get_Updated_When_Update_Called_With_ExistingUser()
        {
            //arrange
            var fakeUser = new User(String.Empty, String.Empty, String.Empty, String.Empty, DateTime.Now);
            _outputHelper.WriteLine(fakeUser.Id.ToString());
            var user = _userService.Add(fakeUser).Result;

            //act
            user.FirstName = "user is modified";
            var updatedUser = _userService.Update(user).Result;

            //assert
            Assert.Equal(updatedUser.FirstName, user.FirstName);
            Assert.Equal(updatedUser, user);
        }
        [Fact]
        public void Verify_Throw_NotSupportedException_When_Update_Called_With_ExistingUser_And_Data_not_Saved()
        {
            var dbContextMock = new Mock<IBloggerDbContext>();

            dbContextMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(0);


            Assert.ThrowsAsync<NotSupportedException>(
                async () =>
                {
                    var userMock = new Mock<User>();
                    var user = _userService.Add(userMock.Object).Result;
                    user.FirstName = "user is modified";
                    await _userService.Update(user);

                });

        }

        #endregion



        #region delete User
        [Fact]
        public void Verify_Throw_ArgumentNullException_When_Delete_Called_With_Null()
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                async () => { await _userService.Delete(null); });

        }
        [Fact]
        public void Verify_Throw_NotSupportedException_When_Delete_Called_With_NonExistingUser()
        {
            var user = new User(String.Empty, String.Empty, String.Empty, String.Empty, DateTime.Now);
            user.Id = 10;
            var exception = Assert.ThrowsAsync<NotSupportedException>(
                async () => await _userService.Delete(user));

            Assert.Contains("exists in our system", exception.Result.Message);


        }

        [Fact]
        public void Verify_User_Get_Deleted_When_Delete_Called_With_ExistingUser()
        {
            //arrange
            var user = new User(String.Empty, String.Empty, String.Empty, String.Empty, DateTime.Now);
            var userAdded = _userService.Add(user).Result;
            _outputHelper.WriteLine(userAdded.ToString());
            var crossVerifyUser = _userService.GetUserById(userAdded.Id).Result;

            _outputHelper.WriteLine(crossVerifyUser.ToString());

            //act
            var deleteStatus = _userService.Delete(crossVerifyUser).Result;

            //assert
            Assert.True(deleteStatus);
            Assert.Null(_userService.GetUserById(userAdded.Id).Result);
        }

        #endregion


    }
}
