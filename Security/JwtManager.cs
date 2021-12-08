using Electronic_Organizer_API.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Electronic_Organizer_API.Security
{
    public class JwtManager
    {
        public static string GenerateJwtToken(EndUser user, IConfiguration configuration)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email), 
                new Claim(JwtClaimTypes.Avatar, user.Avatar),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(configuration["Jwt:ExpireDays"]));

            var token = new JwtSecurityToken(
                configuration["Jwt:ValidAudience"],
                configuration["Jwt:ValidIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public struct JwtClaimTypes
        {
            public const string Avatar = "avatar";
        }
    }
}
