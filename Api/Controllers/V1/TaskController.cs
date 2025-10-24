using Api.Controllers.Base;
using Api.Helper;
using Asp.Versioning;
using Business.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Model.Request.Task;
using System.Net;

namespace Api.Controllers.V1
{
    [AttributeWebAPI]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]

    public class TaskController : ApiBaseController
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;

        }
        [Route("Add")]
        [HttpPost]
        public async Task<IActionResult> AddTask([FromBody] Job request)
        {
            request.CreatedBy = request.NeedsToSendTask ? 0 : UserId;
            var response = await _taskService.AddTask(request,UserId);

            if (response != null)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }
        [Route("Delete")]
        [HttpDelete]

        public async Task<IActionResult> Delete([FromBody] TaskId id)
        {
            var response = await _taskService.DeleteSub(id.Id,id.ScheduledDate,id.IsDeleteFuture);
            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }

        [Route("Tasks")]
        [HttpPost]
        public async Task<IActionResult> Tasks([FromBody] GetTasksRequest request)
        {
            var response = await _taskService.GetTasksWithSubTasks(UserId, request);
            if (response == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound, response);
            }
            return StatusCode((int)HttpStatusCode.OK, response);
        }


        [Route("GetCompletedAndPendingTasks")]
        [HttpPost]
        public async Task<IActionResult> GetCompletedAndPendingTasks([FromBody] GetTasksRequest request)
        {
            var response = await _taskService.GetPendingAndCompletedTask(UserId, request);
            if (response == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound, response);
            }
            return StatusCode((int)HttpStatusCode.OK, response);
        }


        [Route("GetTasksWithAvailability")]
        [HttpPost]
        public async Task<IActionResult> GetTasksWithAvailability([FromBody] GetTasksWithAvailabilityRequest request)
        {
            var response = await _taskService.GetTasksWithAvailability(UserId, request);
            if (response == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound, response);
            }
            return StatusCode((int)HttpStatusCode.OK, response);
        }
    }
}
