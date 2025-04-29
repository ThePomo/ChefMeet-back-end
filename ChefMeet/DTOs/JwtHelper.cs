using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChefMeet.Models;
using Microsoft.IdentityModel.Tokens;

namespace ChefMeet.Helpers;

public static class JwtHelper
{
    public static string GenerateJwtToken(ApplicationUser user, IConfiguration config, IList<string> ruoli)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
        };

        foreach (var ruolo in ruoli)
        {
            claims.Add(new Claim(ClaimTypes.Role, ruolo));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(Convert.ToDouble(config["Jwt:ExpireDays"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
