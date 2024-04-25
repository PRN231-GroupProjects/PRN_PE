using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Repository.Models.Entities;

namespace Repository.Commons.Utils;

public static class Jwt
{
    public static string CreateJwtToken(UserRole member, string issuer, string audience, string key)
    {
        var securityKey = Encoding.ASCII.GetBytes(key);
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.NameIdentifier, member.Id.ToString()),
                new(ClaimTypes.Role, member.UserRole1.ToString()!),
            }),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(securityKey),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}