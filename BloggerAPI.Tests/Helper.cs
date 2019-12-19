using AutoMapper;
using BloggerAPI.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using BloggerAPI.Interfaces;

namespace BloggerAPI.Tests
{
    public static class Helper
    {
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

        public static IMapper Mapper
        {
            get
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new ProfileSetup());
                });
                return config.CreateMapper();
            }
        }
    }
}
