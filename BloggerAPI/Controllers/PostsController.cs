using System;
using System.Threading.Tasks;
using AutoMapper;
using BloggerAPI.DTO.Entities;
using BloggerAPI.DTO.ViewModels;
using BloggerAPI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloggerAPI.Controllers
{
    [Route("api/users/{userId:int}/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public PostsController(IPostService postService, IMapper mapper, IUserService userService)
        {
            _postService = postService;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int userId)
        {
            return Ok(_mapper.Map<PostViewModel[]>(await _postService.GetPostsByUserId(userId)));
        }
        [HttpPost]
        public async Task<IActionResult> Post(int userId, PostViewModel viewModel)
        {

            var user = await _userService.GetUserById(userId);
            if (user == null)
            {

                return NotFound("user does not exists in our system!!");
            }
            var post = _mapper.Map(viewModel, new Post());
            post.CreatedDate = DateTime.Now;
            post.LastModified = null;
            post.CreatedBy = userId;
            post = await _postService.Add(post); // get updated post id
            if (post != null)
            {
                return Created("", _mapper.Map(post, viewModel));
            }
            else
            {
                return BadRequest("post not saved!! try again or concern developer");
            }

        }


        [HttpPut]
        public async Task<IActionResult> Put(int userId, PostViewModel viewModel)
        {
            try
            {
                var user = await _userService.GetUserById(userId);
                var oldPost = await _postService.GetPostById(viewModel.Id);
                if (user == null)
                {
                    return NotFound("user not found in our system!!");
                }
                if (oldPost != null)
                {
                    if (!_postService.CanEdit(oldPost, user))
                    {
                        return Unauthorized($"this user {user.FirstName} {user.LastName} can't edit post {oldPost.PostTitle}!!");
                    }
                    _mapper.Map(viewModel, oldPost);
                    oldPost.LastModified = DateTime.Now;
                    await _postService.Update(oldPost);
                    return Ok(viewModel);
                }
                else
                {
                    return NotFound("post not found in our system!!");
                }
            }
            catch (Exception)
            {
                //set up logger
                return BadRequest("post not saved!! try again or concern developer");
            }
        }



        [HttpDelete]
        [Route("{Id:int}")]
        public async Task<IActionResult> Delete(int userId, int Id)
        {

            var oldPost = await _postService.GetPostById(Id);
            if (oldPost == null)
                return NotFound("post not found in our system!!");


            var user = await _userService.GetUserById(userId);
            if (user == null)
                return NotFound("user not found!!");


            if (!_postService.CanEdit(oldPost, user))
                return Unauthorized("permission denied!!");


            if (await _postService.Delete(oldPost))
            {
                return Ok();
            }

            return BadRequest("post not deleted!!");



        }


    }
}