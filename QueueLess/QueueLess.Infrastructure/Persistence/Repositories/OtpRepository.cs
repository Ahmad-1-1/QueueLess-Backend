using Microsoft.EntityFrameworkCore;
using QueueLess.Application.Interfaces;
using QueueLess.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueLess.Infrastructure.Persistence.Repositories
{
    public class OtpRepository : IOtpRepository
    {
        private readonly QueueLessDbContext _context;

        public OtpRepository(QueueLessDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(OtpRequest otp)
        {
            await _context.OtpRequests.AddAsync(otp);
        }

        public async Task<OtpRequest?> GetLatestValidOtpAsync(Guid userId, string otpCode)
        {
            return await _context.OtpRequests
                .Where(o => o.UserId == userId
                         && o.OtpCode == otpCode
                         && !o.IsUsed
                         && o.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public Task UpdateAsync(OtpRequest otp)
        {
            _context.OtpRequests.Update(otp);
            return Task.CompletedTask;
        }
    }
}
