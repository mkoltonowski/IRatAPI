using System.Net;

namespace Application.Shared;

public record ApplicationError
{
    public ApplicationError(string message)
    {
        Message = message;
    }
        
    public ApplicationError(HttpStatusCode httpStatusCode, string message)
    {
        HttpStatusCode = httpStatusCode;
        Message = message;
    }

    public string Message { get; init; }
    public HttpStatusCode HttpStatusCode { get; init; }
}