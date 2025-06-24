using System.Net;

namespace Luka.Exceptions;

public class ExceptionResponse : Exception
{
    public object Response { get; }
    public HttpStatusCode StatusCode { get; }

    public ExceptionResponse(object response, HttpStatusCode statusCode)
    {
        Response = response;
        StatusCode = statusCode;
    }
}