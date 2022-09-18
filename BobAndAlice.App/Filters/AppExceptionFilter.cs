using BobAndAlice.App.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace BobAndAlice.App.Filters
{
    public class AppExceptionFilter
    {
        public class AppExceptionFilterAttribute : System.Web.Http.Filters.ExceptionFilterAttribute
        {
            public override void OnException(HttpActionExecutedContext context)
            {
                if (context.Exception is AppException)
                {
                    context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
            }
        }
    }
}
