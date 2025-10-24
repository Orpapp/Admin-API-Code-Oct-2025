using Api.Controllers.Base;
using Api.Helper;
using Asp.Versioning;
using Business.IServices;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.Request.Task;
using Shared.Model.Response;
using Shared.Resources;
using System.Net;

namespace Api.Controllers.V1
{
    [AttributeWebAPI]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]

    public class SubTaskController : ApiBaseController
    {
        private readonly ITaskService _taskService;

        public SubTaskController(ITaskService taskService)
        {
            _taskService = taskService;

        }

        [Route("Add")]
        [HttpPost]

        public async Task<IActionResult> Add([FromBody] List<SubTask> request)
        {
            var response = await _taskService.AddSubTask(request);

            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }

        [Route("Update")]
        [HttpPost]

        public async Task<IActionResult> Update([FromBody] TaskRequest request)
        {
            var response = await _taskService.UpdateSubTask(request);

            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }



        [Route("Complete")]
        [HttpPut]
        public async Task<IActionResult> Complete([FromBody] SubTaskId subTaskId)
        {
            var response = await _taskService.CompleteSub(subTaskId.Id);
            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }
       
    }
}
