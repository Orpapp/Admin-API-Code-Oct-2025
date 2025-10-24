using Api.Controllers.Base;
using Api.Helper;
using Asp.Versioning;
using Business.IServices;
using Microsoft.AspNetCore.Mvc;
using Shared.Model.Request.Social;
using System.Net;


namespace Api.Controllers.V1
{
    [AttributeWebAPI]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class FriendController : ApiBaseController
    {
        private readonly IFriendService _friendService;
        public FriendController(IFriendService friendService)
        {
            _friendService = friendService;
        }

        [Route("Find")]
        [HttpPost]
        public async Task<IActionResult> FindFriends([FromBody] Find request)
        {
            request.Sender = UserId;
            var response = await _friendService.FindFriends(request);
            if (response.Data == null)
            {
                return StatusCode((int)HttpStatusCode.NoContent, response);
            }
            return StatusCode((int)HttpStatusCode.OK, response);
        }
        [Route("Request/Send")]
        [HttpPost]
        public async Task<IActionResult> SendRequest([FromBody] SendRequest request)
        {
            request.Sender = UserId;
            var response = await _friendService.SendRequest(request);
            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.NoContent, response);
        }

        [Route("Request/Status")]
        [HttpPost]
        public async Task<IActionResult> RequestAction([FromBody] RequestAction request)
        {
            request.ReceiverId = UserId;
            var response = await _friendService.RequestAction(request);
            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);  
            }
            return StatusCode((int)HttpStatusCode.NoContent, response);
        }

        [Route("Request/Get")]
        [HttpPost]  
        public async Task<IActionResult> GetRequest([FromBody] GetRequest request)
        {
            request.Id = UserId;
            var response = await _friendService.GetRequest(request);
            if (response.Data == null)
            {
                return StatusCode((int)HttpStatusCode.NoContent, response);
            }
            return StatusCode((int)HttpStatusCode.OK, response);
        }

        [Route("Request/Count")]
        [HttpPost]
        public async Task<IActionResult> GetRequestCount()
        {
            var response = await _friendService.GetRequestCount(UserId);
            return StatusCode((int)HttpStatusCode.OK, response);
        }

        [Route("Remove")]
        [HttpPost]
        public async Task<IActionResult> RemoveFriend([FromBody] SendRequest request)
        {
            request.Sender = UserId;
            var response = await _friendService.RemoveFriend(request);
            return StatusCode((int)HttpStatusCode.OK, response);
        }
    }
}
