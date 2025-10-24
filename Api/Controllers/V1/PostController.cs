using Api.Controllers.Base;
using Api.Helper;
using Asp.Versioning;
using Business.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shared.Model.Base;
using Shared.Model.Request.Social;
using Shared.Resources;
using System.Net;


namespace Api.Controllers.V1
{
    [AttributeWebAPI]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class PostController : ApiBaseController
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [Route("Add")]
        [HttpPost] 
        public async Task<IActionResult> AddPost([FromBody] Posts request)
        {
            request.CreatedBy = UserId;
            var response = await _postService.AddPost(request);
            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.BadRequest, response);
          
        }

        [Route("Like")]
        [HttpPost]
        public async Task<IActionResult> LikePost([FromBody] Like request)
        {
            request.LikedBy = UserId;
            var response = await _postService.LikePost(request);
            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
           
            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }

        [Route("Delete")]
        [HttpPost]
        public async Task<IActionResult> DeletePost([FromBody] PostId request)
        {
            var response = await _postService.DeletePost(request);
            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
           
            return StatusCode((int)HttpStatusCode.BadRequest, response);

        }
        [Route("Comment/Delete")]
        [HttpPost]
        public async Task<IActionResult> DeleteComment([FromBody] PostId request)
        {
            var response = await _postService.DeleteComment(request,UserId);
            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
           
            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }

        [Route("Comment")]
        [HttpPost]
        public async Task<IActionResult> PostComment([FromBody] Comment request)
        {
            request.CommentedBy = UserId;
            var response = await _postService.PostComment(request);
            if (response.Data>0)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.BadRequest, response);

        }

        [Route("AllComments")]
        [HttpPost]
        public async Task<IActionResult> GetComments([FromBody] GetComments request)
        {
            var response = await _postService.GetComments(request);
            if (response.Data==null)
            {
                return StatusCode((int)HttpStatusCode.NoContent, response);
            }
            return StatusCode((int)HttpStatusCode.OK, response);
        }



    }
}
