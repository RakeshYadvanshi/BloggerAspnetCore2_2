using AutoMapper;
using BloggerAPI.DTO.Entities;
using BloggerAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAPI.MapperProfiles
{
    public class PostProfile:Profile
    {
        public PostProfile()
        {
            CreateMap<Post, PostViewModel>()
                .ReverseMap();
        }
        
    }
}
