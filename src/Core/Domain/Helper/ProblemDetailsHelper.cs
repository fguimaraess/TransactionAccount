using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Domain.Helper;

public static class ProblemDetailsHelper
{
    public static ProblemDetails CreateProblemDetails(HttpContext context, string title, string detail, int statusCode)
    {
        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };
    }
}

