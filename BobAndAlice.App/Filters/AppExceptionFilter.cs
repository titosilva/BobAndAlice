using BobAndAlice.App.Exceptions;
using BobAndAlice.App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace BobAndAlice.App.Filters
{
    public class AppExceptionFilter : Microsoft.AspNetCore.Mvc.Filters.IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exceptionMessage = context.Exception.InnerException?.Message ?? context.Exception.Message;

            if (context.Exception is AppException)
            {
                context.Result = new ObjectResult(new AppExceptionModel
                {
                    Message = exceptionMessage,
                }) { StatusCode = 418 };

                context.ExceptionHandled = true;
            }
        }
    }
}
