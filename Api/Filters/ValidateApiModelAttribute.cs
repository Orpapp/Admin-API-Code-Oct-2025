using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Shared.Model.Base;

namespace Api.Filters
{
    public class ValidateApiModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new ApiResponse<object>()
                {
                    Data = null,
                    Message = context.ModelState.Values.SelectMany(x => x.Errors).FirstOrDefault()?.ErrorMessage
                });
            }
            base.OnActionExecuting(context);
        }
    }
}
