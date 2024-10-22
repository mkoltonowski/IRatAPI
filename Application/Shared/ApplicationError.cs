using System.Net;

namespace Application.Shared;

public record ApplicationError
{
    public ApplicationError(string messageTranslationKey)
    {
        Message = messageTranslationKey;
    }
        
    public ApplicationError(HttpStatusCode httpStatusCode, string message)
    {
        HttpStatusCode = httpStatusCode;
        Message = message;
    }

    public string Message { get; init; }
    public HttpStatusCode HttpStatusCode { get; init; }
}