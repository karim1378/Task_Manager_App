using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using TaskManagerApp.Application.Exceptions;
using TaskManagerApp.Application.Interfaces;
using TaskManagerApp.Domain.Entities;
using TaskManagerApp.Infrastructure.Repositories;
using Task = System.Threading.Tasks.Task;

namespace TaskManagerApp.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;

        public TokenService(IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository)
        {
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
        }

        public async Task<string> GenerateAccessTokenAsync(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            var tokenString =  new JwtSecurityTokenHandler().WriteToken(token);

            return await Task.FromResult(tokenString);
        }


        public async Task<string> GenerateRefreshTokenAsync(User user)
        {
            var refreshToken = new RefreshToken
            {        
                Token = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                UserId = user.Id,
            };

           var refreshTokenValue =  await _refreshTokenRepository.AddOrUpdateRefreshTokenAsync(refreshToken);

           return refreshTokenValue;

        }


        public async Task<string> GetRefreshTokenAsync(string refreshTokenValue)
        {
            var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(refreshTokenValue);
            if (refreshToken == null || refreshToken.ExpiryDate < DateTime.Now || refreshToken.IsRevoked)
            {
                throw new ServiceException("Invalid or expired refresh token.");
            }

            var user = await _userRepository.FindByIdAsync(refreshToken.UserId);
            if (user == null)
            {
                throw new ServiceException("User not found.");
            }

            var newAccessToken = await GenerateAccessTokenAsync(user);
            return newAccessToken;
        }
    }
}
