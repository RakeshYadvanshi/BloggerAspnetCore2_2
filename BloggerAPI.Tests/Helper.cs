using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using BloggerAPI.DTO.Entities;
using BloggerAPI.DTO.Enum;
using BloggerAPI.Interfaces;

namespace BloggerAPI.Tests
{
    internal class Helper
    {
        private Helper()
        {

        }

        internal class UserEquality : IEqualityComparer<User>
        {
            public bool Equals(User x, User y)
            {
                return CompareProperties<User>(x, y);
                //return x.Id == y.Id &&
                //       x.FirstName == y.FirstName &&
                //       x.LastName == y.LastName;
            }

            public int GetHashCode(User obj)
            {
                throw new NotImplementedException();
            }
        }


        internal static bool CompareProperties<T>(T obj1
            , T obj2)
        {

            var propertiesObj1 = obj1.GetType().GetProperties();
            var propertiesObj2 = obj2.GetType().GetProperties();
            for (var i = 0; i < propertiesObj1.Length; i++)
            {

                if (propertiesObj1[i].GetValue(obj1) != propertiesObj2[i].GetValue(obj2))
                {
                    return false;
                }
            }

            return true;
        }


        internal static class CommentFake
        {
            private static Comment GetDefaultComment()
            {
                return new Comment()
                {
                    CreatedBy = 0,
                    CreatedDate = DateTime.Now,
                    CommentText = "comment text"
                };
            }

            internal static Comment GetDefaultPostComment(int ProfileId,
                int CreatedById)
            {
                var comment = GetDefaultComment();
                comment.CommentOn = CommentOnType.Posts.ToString();
                comment.CommentOnId = ProfileId;
                comment.CreatedBy = CreatedById;
                return comment;
            }

            internal static Comment GetDefaultUserComment(int ProfileId,
                int CreatedById)
            {
                var comment = GetDefaultComment();
                comment.CommentOn = CommentOnType.Users.ToString();
                comment.CommentOnId = ProfileId;
                comment.CreatedBy = CreatedById;

                return comment;
            }



            internal static Comment GetUserCommentExistsInDb(IUserService _userService
                , ICommentService _commentService,
                out User profile, out User creater)
            {
                profile = Helper.UserFake.GetUserExistsInDb(_userService);
                creater = Helper.UserFake.GetUserExistsInDb(_userService);

                var comment = GetDefaultUserComment(profile.Id, creater.Id);
                return _commentService.Add(CommentOnType.Users, comment).Result;
            }

            internal static Comment GetPostCommentExistsInDb(IUserService _userService
                , IPostService _postService, ICommentService _commentService,
                out Post profile, out User creater)
            {
                User user = Helper.UserFake.GetUserExistsInDb(_userService);
                profile = Helper.PostFake.GetPostExistsInDb(user, _postService);

                creater = Helper.UserFake.GetUserExistsInDb(_userService);

                var comment = GetDefaultPostComment(profile.Id, creater.Id);
                return _commentService.Add(CommentOnType.Posts, comment).Result;
            }


        }
        internal static class PostFake
        {

            internal static Post GetDefaultPost()
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

            internal static Post GetPostByUser(User user)
            {
                var post = GetDefaultPost();
                post.CreatedBy = user.Id;
                return post;
            }

            internal static Post GetPostExistsInDb(User user, IPostService _postService)
            {
                var post = GetPostByUser(user);

                return _postService.Add(post).Result;
            }

        }
        internal static class UserFake
        {
            internal static User GetDefaultUser()
            {
                return new User()
                {
                    LastName = "lastname",
                    FirstName = "first name",
                    CreatedDate = DateTime.Now,
                    Email = "rk@gmail.com"
                };
            }

            internal static User GetUserExistsInDb(IUserService _userService)
            {
                var user = GetDefaultUser();
                return _userService.Add(user).Result;
            }
        }
    }
}
