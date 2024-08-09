using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagerApp.Application.Interfaces;
using TaskManagerApp.Domain.Entities;
using TaskManagerApp.Infrastructure.Persistence;
using Task = System.Threading.Tasks.Task;

namespace TaskManagerApp.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> AddOrUpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            var existingToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == refreshToken.UserId);
            if (existingToken == null)
            {
                await _context.RefreshTokens.AddAsync(refreshToken);

            }
            else
            {
                existingToken.Token = refreshToken.Token;
                existingToken.ExpiryDate = refreshToken.ExpiryDate;
                existingToken.IsRevoked = refreshToken.IsRevoked;
                _context.RefreshTokens.Update(existingToken);
            }
            await _context.SaveChangesAsync();
            return refreshToken.Token;
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string refreshTokenValue)
        {
            return await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Token == refreshTokenValue);
        }
    }
}