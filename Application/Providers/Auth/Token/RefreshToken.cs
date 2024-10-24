namespace Application.Providers.Auth.Token;

public class RefreshToken : BearerToken
{
    public required DateTime Expires { get; set; }
    public required DateTime Created { get; set; }
}