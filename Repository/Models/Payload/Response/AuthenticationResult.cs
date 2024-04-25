using Microsoft.IdentityModel.Tokens;

namespace Repository.Models.Payload.Response;

public class AuthenticationResult
{
    public string Token { get; set; }
}