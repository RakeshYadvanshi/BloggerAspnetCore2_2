using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BloggerAPI.Data;
using BloggerAPI.DTO.Entities;
using BloggerAPI.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloggerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private BloggerDbContext _dbContext;
        private IMapper _mapper;
        public CommentsController(BloggerDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_mapper.Map<CommentViewModel[]>( _dbContext.Comments.ToList()));
        }
        [HttpPost]
        public async Task<IActionResult> Post(CommentViewModel viewModel)
        {

            var comt = _mapper.Map(viewModel, new Comment());
            await _dbContext.Comments.AddAsync(comt);

            if (await _dbContext.SaveChangesAsync() > 0)
            {
                return Created("", _mapper.Map(comt, viewModel));
            }
            else
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    "comment not saved!! try again or concern developer");
            }

        }
    }
}