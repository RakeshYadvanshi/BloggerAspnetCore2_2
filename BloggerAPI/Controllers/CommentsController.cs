using System;
using System.Threading.Tasks;
using AutoMapper;
using BloggerAPI.DTO.Entities;
using BloggerAPI.DTO.Enum;
using BloggerAPI.DTO.ViewModels;
using BloggerAPI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloggerAPI.Controllers
{
    [Route("api/{entityType:regex(Posts|Users)}/{entityId:int}/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService, IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string entityType, int entityId)
        {
            if (Enum.TryParse<CommentOnType>(entityType, out var commentOn))
            {
                switch (commentOn)
                {
                    case CommentOnType.Posts:
                        return Ok(_mapper.Map<Comment[]>(await _commentService.GetCommentsByPostId(entityId)));
                    case CommentOnType.Users:
                        return Ok(_mapper
                     .Map<Comment[]>(await _commentService
                                             .GetCommentsByUserId(entityId)));
                    default:
                        return this.StatusCode(StatusCodes.Status501NotImplemented, $" {commentOn.ToString()} does not support comments");
                }
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPost]
        [Route("{userId:int}")]
        public async Task<IActionResult> Post(string entityType, int entityId,
            CommentViewModel viewModel,int userId)
        {

            
            if (Enum.TryParse<CommentOnType>(entityType, out var commentOn))
            {
                var user = await _userService.GetUserById(userId);
                if (user == null)
                {
                    return this.StatusCode(StatusCodes.Status400BadRequest,
                              $"User does not exists in our system!!");
                }

                var comment = _mapper.Map(viewModel, new Comment());
                comment.CreatedDate = DateTime.Now;
                comment.LastModified = null;
                comment.CommentOn = commentOn.ToString();
                comment.CommentOnId = entityId;
                comment.CreatedBy = userId;
                comment = await _commentService.Add(commentOn,comment); // get updated post id
                if (comment != null)
                {
                    return Created("", _mapper.Map(comment, viewModel));
                }
                else
                {
                    return this.StatusCode(StatusCodes.Status500InternalServerError,
                        "user not saved!! try again or concern developer");
                }
            }
            else
            {
                return NotFound();
            }
        }




        [HttpPut]
        [Route("{userId:int}")]
        public async Task<IActionResult> Put(string entityType, int entityId,
            CommentViewModel viewModel, int userId)
        {
            try
            {
                if (Enum.TryParse<CommentOnType>(entityType, out var commentOn))
                {

                    var user = await _userService.GetUserById(userId);
                    if (user == null)
                    {
                        return this.StatusCode(StatusCodes.Status400BadRequest,
                                  $"User does not exists in our system!!");
                    }
                    var oldComment = await _commentService.GetCommentById(viewModel.Id);
                    if (oldComment != null)
                    {
                        if (!_commentService.CanEdit(oldComment, user))
                        {
                            return this.StatusCode(StatusCodes.Status203NonAuthoritative,
                                   $"this user {user.FirstName} {user.LastName} can't edit comment!!");
                        }
                        // overriding createdby send from client from accidently update exception can be thrown instead
                        viewModel.CreatedBy = oldComment.CreatedBy; 

                        _mapper.Map(viewModel, oldComment);
                        oldComment.CommentOnId = entityId;
                        oldComment.LastModified = DateTime.Now;
                        await _commentService.Update(commentOn, oldComment);
                        return Ok(viewModel);
                    }
                    else
                    {
                        return this.StatusCode(StatusCodes.Status404NotFound,
                               "post not found in our system!!");
                    }

                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                //set up logger
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                            "comment not saved!! try again or concern developer");
            }
        }




        [HttpDelete]
        [Route("{userId:int}/{commentId:int}")]
        public async Task<IActionResult> Delete(string entityType, int entityId, int userId, int commentId)
        {
            if (Enum.TryParse<CommentOnType>(entityType, out var commentOn))
            {

                var oldComment = await _commentService.GetCommentById(commentId);
                var user = await _userService.GetUserById(userId);

                if (oldComment.CommentOnId != entityId)
                {
                    return this.StatusCode(StatusCodes.Status400BadRequest,
                             "comment can't be moved between entities !!");
                }

                if (oldComment.CommentOn != commentOn.ToString())
                {
                    return this.StatusCode(StatusCodes.Status400BadRequest,
                             "comment can't be moved between entities !!");
                }
                if (!_commentService.CanEdit(oldComment, user))
                {
                    return this.StatusCode(StatusCodes.Status400BadRequest,
                             "this user can't delete comment !!");
                }
                if (oldComment != null)
                {
                    if (await _commentService.Delete(oldComment))
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
                           "post not found in our system!!");
                }

            }
            else
            {
                return NotFound();
            }

        }



    }
}