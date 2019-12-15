using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BloggerAPI.Data;
using Microsoft.EntityFrameworkCore;
using System;
using StructureMap;
using AutoMapper;
using BloggerAPI.Interfaces;
using BloggerAPI.Services;

namespace BloggerAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. 
        //Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                    .AddControllersAsServices()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAutoMapper(typeof(Startup));
            services.AddDbContext<BloggerDbContext>(
                opt => opt.UseInMemoryDatabase("Blogger"));


            return ConfigureIoC(services);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }




        private static IServiceProvider ConfigureIoC(IServiceCollection services)
        {
            var container = new Container();
            container.Configure(config =>
            {
                config.Scan(_ =>
                {
                    _.AssemblyContainingType(typeof(IUserService));
                    _.AssemblyContainingType(typeof(UserService));
                    _.AssemblyContainingType(typeof(Startup));// register services only startup class project
                    _.WithDefaultConventions();
                });
                config.For<BloggerDbContext>().Use<BloggerDbContext>().Singleton();// to make life of in Memory data till Site running
                config.Populate(services); // use to register all the framework services to structuremap 
            });

            return container.GetInstance<IServiceProvider>();
        }
    }
}
