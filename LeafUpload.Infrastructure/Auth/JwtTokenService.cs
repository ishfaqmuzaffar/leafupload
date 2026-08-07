using LeafUpload.Core.Abstractions;
using LeafUpload.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LeafUpload.Infrastructure.Auth
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string IssueToken(Farmer farmer)
        {
            var secretKey = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidOperationException("Jwt:SecretKey is not configured.");

            var expiryDays = _configuration.GetValue<int?>("Jwt:ExpiryDays") ?? 30;

            // Same claim shape the cookie scheme uses (see AccountController.SignInFarmerAsync)
            // so any existing logic reading these claims works unchanged for bearer requests.
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, farmer.Id.ToString()),
                new(ClaimTypes.Name, farmer.Username),
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(expiryDays),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
