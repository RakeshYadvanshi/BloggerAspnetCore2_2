using System;
using System.Threading.Tasks;
using AutoMapper;
using BloggerAPI.DTO.Entities;
using BloggerAPI.Interfaces;
using BloggerAPI.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloggerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(_mapper.Map<UserViewModel[]>(await _userService.GetUsers()));
        }
        [HttpPost]
        public async Task<IActionResult> Post(UserViewModel viewModel)
        {
            var user = _mapper.Map(viewModel, new User());
            user.CreatedDate = DateTime.Now;
            user.LastModified = null;
            if (await _userService.Add(user) != null)
            {
                return Created("", _mapper.Map(user, viewModel));
            }
            else
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                    "user not saved!! try again or concern developer");
            }

        }


        [HttpPut]
        public async Task<IActionResult> Put(UserViewModel viewModel)
        {
            try
            {
                var oldUser = await _userService.GetUserById(viewModel.Id);
                if (oldUser != null)
                {
                    _mapper.Map(viewModel, oldUser);
                    oldUser.LastModified = DateTime.Now;
                    await _userService.Update(oldUser);
                    return Ok(viewModel);
                }
                else
                {
                    return this.StatusCode(StatusCodes.Status404NotFound,
                           "user not found in our system!!");
                }
            }
            catch (Exception)
            {
                //set up logger
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                            "user not saved!! try again or concern developer");
            }
        }

        [HttpDelete]
        [Route("{Id:int}")]
        public async Task<IActionResult> Delete(int Id)
        {

            var oldUser = await _userService.GetUserById(Id);
            if (oldUser != null)
            {
                if (await _userService.Delete(oldUser))
                {
                    return Ok();
                }
                else
                {
                    return this.StatusCode(StatusCodes.Status500InternalServerError,
                         "user not deleted!! try again or concern developer");
                }
            }
            else
            {
                return this.StatusCode(StatusCodes.Status404NotFound,
                       "user not found in our system!!");
            }
        }
    }
}