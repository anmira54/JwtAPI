using System;
using Core.DTOs;
using Core.Models;
using System.Security.Claims;
using System.Text;
using System.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Core.Interfaces;
using System.Text.Json;

namespace Core.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly AppSettings _appSettings;
        public LoginRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public bool IsValidLogin(LoginRequest login, User user)
        {
            return CheckPassword(login.Password, user.PasswordHash);
        }

        public LoginSuccess GetJwt(User user)
        {

            byte[] bytesKey = Encoding.UTF8.GetBytes(_appSettings.JwtSecret);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity
                (
                    GetUserClaims(user)
                ),
                Expires = DateTime.UtcNow.AddDays(14),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(bytesKey), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _appSettings.Issuer,
                Audience = _appSettings.Audience,
            };


            JwtSecurityTokenHandler tokenHandler = new();

            LoginSuccess jwt = new()
            {
                Token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor))
            };

            return jwt;
        }

        private static bool CheckPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        public static Claim[] GetUserClaims(User user)
        {
            List<Claim> claims = new()
                            {
                                new Claim(ClaimTypes.Name, user.UserName),
                                new Claim(ClaimTypes.Role, user.Role.Name),
                            };

            foreach (Permission permission in user.Role.Permissions)
            {

                claims.Add(new Claim("permissions", permission.Name));
            }


            return claims.ToArray();
        }
    }
}

