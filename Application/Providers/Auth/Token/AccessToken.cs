namespace Application.Providers.Auth.Token;

public class AccessToken : BearerToken
{
    public required string TokenId {get; set;}
}