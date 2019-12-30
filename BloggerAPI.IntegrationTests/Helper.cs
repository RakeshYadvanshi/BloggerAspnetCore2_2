using System;
using AutoMapper;
using BloggerAPI.Data;
using BloggerAPI.DTO.MapperProfiles;
using BloggerAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BloggerAPI.IntegrationTests
{
    public class Helper
    {
        public static IMapper Mapper
        {
            get
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new CommonSetup());
                });
                return config.CreateMapper();
            }
        }

        public static IBloggerDbContext DbContext
        {
            get
            {
                var builder = new DbContextOptionsBuilder<BloggerDbContext>();
                builder.UseInMemoryDatabase($"Tests{Guid.NewGuid()}");
                var options = builder.Options;
                var dbContext = new BloggerDbContext(options);
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();

                return dbContext;
            }
        }

    }
}
