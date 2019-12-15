using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BloggerAPI.Data;
using BloggerAPI.DTO.Entities;
using BloggerAPI.Interfaces;
using BloggerAPI.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloggerAPI.Controllers
{
    [Route("api/users/{userId:int}/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private IPostService _postService;
        private IMapper _mapper;
        public PostsController(IPostService postService, IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int userId)
        {
            return Ok(_mapper.Map<Post[]>(_postService.GetPostsByUserId(userId)));
        }
        [HttpPost]
        public async Task<IActionResult> Post(int userId, PostViewModel viewModel)
        {
            var post = _mapper.Map(viewModel, new Post());
            post = await _postService.Add(post); // get updated post id
            if (post != null)
            {
                return Created("", _mapper.Map(post, viewModel));
            }
            else
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    "user not saved!! try again or concern developer");
            }

        }


        [HttpPut]
        public async Task<IActionResult> Put(int userId, PostViewModel viewModel)
        {
            try
            {
                var oldPost = await _postService.GetPostById(viewModel.Id);
                if (oldPost != null)
                {
                    _mapper.Map(viewModel, oldPost);
                    await _postService.Update(oldPost);
                    return Ok(viewModel);
                }
                else
                {
                    return this.StatusCode(StatusCodes.Status404NotFound,
                           "post not fount in our system!!");
                }
            }
            catch (Exception)
            {
                //set up logger
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                            "post not saved!! try again or concern developer");
            }
        }



        [HttpDelete]
        [Route("{Id:int}")]
        public async Task<IActionResult> Delete(int userId, int Id)
        {

            var oldUser = await _postService.GetPostById(Id);
            if (oldUser != null)
            {
                if (await _postService.Delete(oldUser))
                {
                    return Ok();
                }
                else
                {
                    return this.StatusCode(StatusCodes.Status500InternalServerError,
                         "post not deleted!! try again or concern developer");
                }
            }
            else
            {
                return this.StatusCode(StatusCodes.Status404NotFound,
                       "post not fount in our system!!");
            }
        }
    }
}