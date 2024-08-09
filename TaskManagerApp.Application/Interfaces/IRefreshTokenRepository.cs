using TaskManagerApp.Domain.Entities;
using Task = System.Threading.Tasks.Task;
namespace TaskManagerApp.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<string> AddOrUpdateRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken> GetRefreshTokenAsync(string refreshTokenValue);
    }
}
