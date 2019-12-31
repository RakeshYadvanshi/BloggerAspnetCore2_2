using BloggerAPI.DTO.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using BloggerAPI.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace BloggerAPI.Tests
{

    public class UserServiceShould : IClassFixture<ContainerFixture>
    {
        private readonly IUserService _userService;
        private readonly ITestOutputHelper _outputHelper;

        public UserServiceShould(ITestOutputHelper outputHelper, ContainerFixture container)
        {
            _outputHelper = outputHelper;
            //SUT
            _userService = container.GetContainer.GetInstance<IUserService>();
        }

        #region GetUsers
        [Fact]
        public void Verify_Give_Users_When_GetUsers_Called()
        {
            //arrange
            var user = Helper.UserFake.GetDefaultUser();
            var userAdded = _userService.Add(user).Result;


            //act
            var result = _userService.GetUsers().Result;


            //assert
            Assert.True(result.Any());


        }


        #endregion

        #region GetUser

        [Fact]
        public void Verify_Give_User_When_GetUserById_Called_For_ExistingUser()
        {
            //arrange
            var user = Helper.UserFake.GetDefaultUser();
            var userAdded = _userService.Add(user).Result;

            //act
            var result = _userService.GetUserById(user.Id).Result;

            //assert
            Assert.NotNull(result);
            Assert.True(result.Id == user.Id);
            Assert.True(result.FirstName == user.FirstName);
            Assert.True(result.LastName == user.LastName);

        }
        [Fact]
        public void Verify_Give_Null_When_GetUserById_Called_For_NonExistingUser()
        {
            //arrange
            var userid = 10;

            //act
            var result = _userService.GetUserById(userid).Result;

            //assert
            Assert.Null(result);
        }

        #endregion

        #region add

        [Fact]
        public void Verify_Add_NewUser_When_Add_Called_With_NotNullUser()
        {
            //arrange
            var user = Helper.UserFake.GetDefaultUser();

            //act
            var userAdded = _userService.Add(user).Result;

            //assert

            var users = _userService.GetUsers().Result;
            var dbUser = _userService.GetUserById(user.Id).Result;

            Assert.True(users.Any());
            Assert.Equal(user.Id, dbUser.Id);
            Assert.Equal(user.LastName, dbUser.LastName);
        }
        [Fact]
        public void Verify_Throw_ArgumentNullException_When_Add_Called_With_Null()
        {
            //assert
            Assert.ThrowsAsync<ArgumentNullException>(
                async () =>
                {
                    //act
                    await _userService.Add(null);
                });

        }

        #endregion

        #region Update User
        [Fact]
        public void Verify_Throw_ArgumentNullException_When_Update_Called_With_Null()
        {
            //assert
            Assert.ThrowsAsync<ArgumentNullException>(
                async () =>
                {
                    //act
                    await _userService.Update(null);
                });

        }
        [Fact]
        public void Verify_Throw_NotSupportedException_When_Update_Called_With_NonExistingUser()
        {
            //arrange
            var user = Helper.UserFake.GetDefaultUser();

            //assert
            var exception = Assert.ThrowsAsync<NotSupportedException>(
                   async () =>
                   {
                       //act
                       await _userService.Update(user);
                   });
            Assert.Contains("not exists in our system", exception.Result.Message,
                StringComparison.OrdinalIgnoreCase);
        }



        [Fact]
        public void Verify_User_Get_Updated_When_Update_Called_With_ExistingUser()
        {
            //arrange
            var user = Helper.UserFake.GetDefaultUser();
            var userAdded = _userService.Add(user).Result;

            //act
            user.FirstName = "first name is modified";
            user.LastName = "last name user is modified";
            var updatedUser = _userService.Update(user).Result;

            //assert
            Assert.Equal(updatedUser.FirstName, user.FirstName);
            Assert.Equal(updatedUser.LastName, user.LastName);
            Assert.Equal(updatedUser.Id, user.Id);
        }
        [Fact]
        public void Verify_Throw_NotSupportedException_When_Update_Called_With_ExistingUser_And_Data_not_Saved()
        {
            //arrange
            var user = Helper.UserFake.GetDefaultUser();
            var userAdded = _userService.Add(user).Result;
            user.FirstName = "user is modified";


            //assert
            Assert.ThrowsAsync<NotSupportedException>(
                async () =>
                {
                    //act
                    await _userService.Update(user);

                });

        }

        #endregion



        #region delete User
        [Fact]
        public void Verify_Throw_ArgumentNullException_When_Delete_Called_With_Null()
        {
            //assert
            Assert.ThrowsAsync<ArgumentNullException>(
                async () =>
                {
                    //act
                    await _userService.Delete(null);
                });

        }
        [Fact]
        public void Verify_Throw_NotSupportedException_When_Delete_Called_With_NonExistingUser()
        {
            //arrange
            var user = Helper.UserFake.GetDefaultUser();
            user.Id = 10;

            //assert
            var exception = Assert.ThrowsAsync<NotSupportedException>(
                async () =>
                {
                    //act
                    await _userService.Delete(user);
                });
            Assert.Contains("not exists in our system", exception.Result.Message,
                StringComparison.OrdinalIgnoreCase);


        }

        [Fact]
        public void Verify_User_Get_Deleted_When_Delete_Called_With_ExistingUser()
        {
            //arrange
            var user = Helper.UserFake.GetDefaultUser();
            var userAdded = _userService.Add(user).Result;

            //act
            var deleteStatus = _userService.Delete(user).Result;

            //assert
            var dbUser = _userService.GetUserById(userAdded.Id).Result;
            Assert.True(deleteStatus);
            Assert.Null(dbUser);
        }

        #endregion


      


    }
}
