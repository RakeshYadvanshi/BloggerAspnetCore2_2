using AutoMapper;
using BloggerAPI.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BloggerAPI.Tests
{
    public static class Helper
    {
        public static BloggerDbContext DbContext
        {
            get
            {
                DbContextOptions<BloggerDbContext> options;
                var builder = new DbContextOptionsBuilder<BloggerDbContext>();
                builder.UseInMemoryDatabase("Tests");
                options = builder.Options;
                BloggerDbContext dbContext = new BloggerDbContext(options);
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
