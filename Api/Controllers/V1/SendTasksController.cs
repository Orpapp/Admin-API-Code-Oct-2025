using Api.Controllers.Base;
using Api.Helper;
using Asp.Versioning;
using Business.IServices;
using Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Model.Request.SendTasks;
using Shared.Model.Request.Task;
using System.Net;

namespace Api.Controllers.V1
{
    [AttributeWebAPI]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class SendTasksController : ApiBaseController
    {
        private readonly ISendTasksService _sendTasksService;
        public SendTasksController(ISendTasksService sendTasksService)
        {
            _sendTasksService = sendTasksService;
        }


        [HttpPost]
        [Route("SendTask")]
        public async Task<IActionResult> SendTaskToFriend(SendTask request)
        {
            var result = await _sendTasksService.SendTaskToFriend(request, UserId);
            if (!result.Data)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, result);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.OK, result);
            }
        }

        [HttpPost]
        [Route("AcceptTask")]
        public async Task<IActionResult> AcceptReceivedTask(AcceptTask request)
        {
            var result = await _sendTasksService.AcceptReceivedTask(request, UserId);
            if (!result.Data)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, result);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.OK, result);
            }
        }

        [Route("ReceivedTasks")]
        [HttpGet]
        public async Task<IActionResult> ReceivedTasks()
        {
            var response = await _sendTasksService.GetReceivedTasksWithSubTasks(UserId);
            if (response == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound, response);
            }
            return StatusCode((int)HttpStatusCode.OK, response);
        }
    }
}
