using AutoMapper;
using BloggerAPI.DTO.Entities;
using BloggerAPI.DTO.ViewModels;

namespace BloggerAPI.MapperProfiles
{
    public class ProfileSetup : Profile
    {
        public ProfileSetup()
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
