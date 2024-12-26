using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyTrips.Presentation.Filters;

public class ProblemHeaderFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult { StatusCode: >= 400 })
            context.HttpContext.Response.Headers.ContentType = "application/problem+json; charset=utf-8";
    }
}