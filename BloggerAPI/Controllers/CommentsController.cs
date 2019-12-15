using System;
using System.Collections;
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
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        //private IMapper _mapper;
        //private ICommentService _commentService;
        //public CommentsController(ICommentService commentService, IMapper mapper)
        //{
        //    _mapper = mapper;
        //    _commentService = commentService;
        //}

        //[HttpGet]
        //public IActionResult Get()
        //{
        //    return Ok(_mapper.Map<CommentViewModel[]>( _commentService.GetComments()));
        //}
        //[HttpPost]
        //public async Task<IActionResult> Post(CommentViewModel viewModel)
        //{
        //    var comt = _mapper.Map(viewModel, new Comment());
        //    await _dbContext.Comments.AddAsync(comt);

        //    if (await _dbContext.SaveChangesAsync() > 0)
        //    {
        //        return Created("", _mapper.Map(comt, viewModel));
        //    }
        //    else
        //    {
        //        return this.StatusCode(StatusCodes.Status500InternalServerError,
        //            "comment not saved!! try again or concern developer");
        //    }

        //}


        //[HttpPut]
        //public async Task<IActionResult> Put(CommentViewModel viewModel)
        //{

        //    var oldComment = _dbContext.Comments.Where(x => x.Id == viewModel.Id).FirstOrDefault();
        //    if (oldComment != null)
        //    {
        //        _mapper.Map(viewModel, oldComment);
        //        if (await _dbContext.SaveChangesAsync() > 0)
        //        {
        //            return Ok(viewModel);
        //        }
        //        else
        //        {
        //            return this.StatusCode(StatusCodes.Status500InternalServerError,
        //                 "comment not saved!! try again or concern developer");
        //        }
        //    }
        //    else
        //    {
        //        return this.StatusCode(StatusCodes.Status404NotFound,
        //               "comment not fount in our system!!");
        //    }
        //}



        //[HttpDelete]
        //[Route("{Id:int}")]
        //public async Task<IActionResult> Delete(int Id)
        //{

        //    var oldComment = _dbContext.Comments.Where(x => x.Id == Id).FirstOrDefault();
        //    if (oldComment != null)
        //    {
        //        _dbContext.Comments.Remove(oldComment);
        //        if (await _dbContext.SaveChangesAsync() > 0)
        //        {
        //            return Ok();
        //        }
        //        else
        //        {
        //            return this.StatusCode(StatusCodes.Status500InternalServerError,
        //                 "comment not saved!! try again or concern developer");
        //        }
        //    }
        //    else
        //    {
        //        return this.StatusCode(StatusCodes.Status404NotFound,
        //               "comment not fount in our system!!");
        //    }
        //}
    }
}