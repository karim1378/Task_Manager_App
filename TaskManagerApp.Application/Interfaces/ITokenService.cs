using TaskManagerApp.Domain.Entities;

namespace TaskManagerApp.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateAccessTokenAsync(User user);
        Task<string> GenerateRefreshTokenAsync(User user);
        Task<string> GetRefreshTokenAsync(string refreshTokenValue);
    }
}
