using System.Net;
using CSharpFunctionalExtensions;
using Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class BaseController : ControllerBase
    {
        private IMediator _mediator = null!;

        protected IMediator Mediator 
            => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
        
        private ActionResult MapErrorToHttpResult(ApplicationError error)
        {
            return MapToActionResult(error.HttpStatusCode, error.Message);
        }
        
        protected  ActionResult HttpResult<T>(IResult<T, ApplicationError> result)
        {
            return result.IsSuccess ? Ok(result.Value) : MapErrorToHttpResult(result.Error);
        }
        
        private ActionResult MapToActionResult(HttpStatusCode httpStatusCode, string errorMessage) => httpStatusCode switch
        {
            HttpStatusCode.BadRequest => BadRequest(errorMessage),
            HttpStatusCode.Unauthorized => Unauthorized(errorMessage),
            HttpStatusCode.NotFound => NotFound(errorMessage),
            HttpStatusCode.NoContent => NoContent(),
            HttpStatusCode.OK => Ok(errorMessage),
            _ => StatusCode((int)httpStatusCode, errorMessage)
        };
    }
}
