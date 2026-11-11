namespace OsbErpLite.Domain.Common;

public class AuthenticationException : Exception
{
    public AuthenticationException(string message) : base(message) { }
}
