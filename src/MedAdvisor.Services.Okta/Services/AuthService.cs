using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.Extensions.Options;
using Google.Apis.Auth;
using MedAdvisor.Models;
using System.Security.Claims;
using MedAdvisor.Services.Okta.Interfaces;

namespace MedAdvisor.Services.Okta.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly GoogleAuthSettings _googleAuthSettings;

        public AuthService(
            IConfiguration config,
            IOptionsSnapshot<GoogleAuthSettings> googleAuthSettings
            )
        {
            _googleAuthSettings = googleAuthSettings.Value;
            _configuration = config;

        }

        public string CreateToken(User user)
        {
            var claims = new List<Claim>{
             new Claim("Id",user.UserId.ToString()),
             new Claim(ClaimTypes.Email, user.Email),
             new Claim(ClaimTypes.Name, user.FullName),
        };

            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Secret").Value);
            var SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
              claims: claims,
              expires: DateTime.Now.AddDays(30),
              signingCredentials: SigningCredentials
          );

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return jwtToken;
        }


        public Guid GetId(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Secret").Value);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var id = jwtToken.Claims.First(x => x.Type == "Id").Value;
            var user_id = new Guid(id);

            return user_id;
        }

        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string token)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { _googleAuthSettings.ClientId }
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
            return payload;
        }


    }
}
