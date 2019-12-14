using AutoMapper;
using BloggerAPI.DTO.Entities;
using BloggerAPI.ViewModels;

namespace BloggerAPI.MapperProfiles
{
    public class CommentProfile: Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentViewModel>()
                .ReverseMap();
        }
    }
}
