namespace AssetsManagerApi.Application.Exceptions;

public class TokenExpiredException : Exception
{
    public TokenExpiredException(string message)
        : base(message) { }

    public TokenExpiredException(string message, Exception innerException)
        : base(message, innerException) { }
}
