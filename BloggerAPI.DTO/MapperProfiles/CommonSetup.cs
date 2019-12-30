using AutoMapper;
using BloggerAPI.DTO.Entities;
using BloggerAPI.DTO.ViewModels;

namespace BloggerAPI.DTO.MapperProfiles
{
    public class CommonSetup:Profile
    {
        public CommonSetup()
        {
            CreateMap<Comment, CommentViewModel>()
               .ReverseMap();
            CreateMap<User, UserViewModel>()
               .ReverseMap();
            CreateMap<Post, PostViewModel>()
               .ReverseMap();
        }
    }
}
