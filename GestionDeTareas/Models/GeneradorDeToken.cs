using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GestionDeTareas.Models
{
    public class GeneradorDeToken
    {

            private readonly IConfiguration _configuration;

            // Constructor que recibe IConfiguration
            public GeneradorDeToken(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public string GenerateJwtToken(string username)
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Name, username)
                }),
                    Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpirationInMinutes"])),
                    Issuer = jwtSettings["Issuer"],
                    Audience = jwtSettings["Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }

        }
}
