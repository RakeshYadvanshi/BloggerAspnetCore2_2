using System;
using AutoMapper;
using BloggerAPI.Data;
using BloggerAPI.DTO.MapperProfiles;
using BloggerAPI.Interfaces;
using BloggerAPI.Services;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Moq;
using StructureMap;

namespace BloggerAPI.Tests
{
    public class StructureMapRegistry : Registry
    {
        public StructureMapRegistry()
        {
            

            //set up mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new CommonSetup());
            });
            var mapper = config.CreateMapper();
            For<IMapper>().Use(mapper);

            Scan((Scan) =>
            {
                Scan.WithDefaultConventions();
                Scan.AssemblyContainingType(typeof(StructureMapRegistry));
                Scan.AssemblyContainingType(typeof(IUserService));
                Scan.AssemblyContainingType(typeof(UserService));
            });

            //set up dbcontext
            var builder = new DbContextOptionsBuilder<BloggerDbContext>();
            builder.UseInMemoryDatabase($"Tests{Guid.NewGuid()}");
            var options = builder.Options;
            var dbContext = new BloggerDbContext(options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            For<IBloggerDbContext>().Use(dbContext);

           
        }
    }
}
