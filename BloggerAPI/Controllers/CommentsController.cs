using System;
using System.Threading.Tasks;
using BloggerAPI.Data;
using BloggerAPI.DTO.Entities;
using BloggerAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BloggerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private BloggerDbContext _dbContext;
        public CommentsController(BloggerDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public IActionResult Get()
        {
            return Ok(_dbContext.Comments);
        }

        public async Task<IActionResult> Post(CommentViewModel viewModel)
        {
            Comment comment = new Comment()
            {
                Id = viewModel.Id,
                CommentOn = viewModel.CommentOn,
                CommentText = viewModel.CommentText,
                CreateDate = DateTime.Now,
                CreatedBy = viewModel.CreatedBy
            };
            _dbContext.Comments.Add(comment);

            await _dbContext.SaveChangesAsync();

            return Created("", comment);
        }
    }
}