using Api.Controllers.Base;
using Api.Helper;
using Asp.Versioning;
using Business.IServices;
using Microsoft.AspNetCore.Mvc;
using Shared.Model.Base;
using Shared.Model.Request.Group;
using Shared.Model.Request.Social;
using Shared.Model.Response;
using System.Net;


namespace Api.Controllers.V1
{
    [AttributeWebAPI]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class GroupController : ApiBaseController
    {
        private readonly IGroupService _groupService;
        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [Route("Find")]
        [HttpPost]
        public async Task<IActionResult> GetGroups([FromBody] GetGroups request)
        {
            request.Id = UserId;
            var response = await _groupService.GetGroups(request);
            if (response.Data == null)
            {
                return StatusCode((int)HttpStatusCode.NoContent, response);
            }
            return StatusCode((int)HttpStatusCode.OK, response);
        }

        [Route("AddUpdate")]
        [HttpPost]
        public async Task<IActionResult> AddUpdateGroup([FromBody] Group request)
        {
            request.CreatedBy = UserId;
            var response = await _groupService.AddUpdateGroup(request);
            if (response.Data>0)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.NoContent, response);
           
        }

        [Route("Posts")]
        [HttpPost]
        public async Task<IActionResult> GetGroupPosts([FromBody] GroupPosts request)
        {
            request.Id = UserId;
            var response = await _groupService.GetGroupPosts(request);
            if (response.Data != null)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.NoContent, response);

        }

        [Route("Delete")]
        [HttpPost]
        public async Task<IActionResult> DeleteGroup([FromBody] GroupId request)
        {
            var response = await _groupService.DeleteGroup(request);
            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.NoContent, response);

        }
        
        [Route("Left")]
        [HttpPost]
        public async Task<IActionResult> LeftGroup([FromBody] Participants request)
        {
            var response = await _groupService.LeftGroup(request,UserId);
            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.NoContent, response);
        }

        [Route("Participants")]
        [HttpPost]
        public async Task<IActionResult> GetParticipants([FromBody] GroupId request)
        {
            var response = await _groupService.GetParticipants(request);
            if (response.Data != null)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.NoContent, response);
        }

        [Route("GetGroupWithParticipants")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GroupDetailsWithParticipants>))]
        public async Task<IActionResult> GetGroupWithParticipants(GroupId request)
        {
            var result = await _groupService.GetGroupWithParticipants(request,UserId);
            return StatusCode((int)HttpStatusCode.OK, result);
        }

        [Route("DeleteGroupParticipant")]
        [HttpPost]
        public async Task<IActionResult> DeleteGroupParticipant(DeleteGroupParticipantRequestModel request)
        {
            var response = await _groupService.DeleteGroupParticipant(request);
            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.NoContent, response);
        }

        [Route("GetNotAddedParticipants")]
        [HttpPost]
        public async Task<IActionResult> GetNotAddedParticipants(GetNotAddedParticipantsRequestModel request)
        {
            var response = await _groupService.GetNotAddedParticipants(request,UserId);
            if (response.Data != null)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.NoContent, response);
        }

        [Route("AddParticipants")]
        [HttpPost]
        public async Task<IActionResult> AddParticipants(AddParticipantsRequestModel request)
        {
            var response = await _groupService.AddParticipants(request);
            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.NoContent, response);

        }


    }
}
