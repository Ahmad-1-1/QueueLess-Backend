using QueueLess.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueLess.Application.Interfaces
{
    public interface IOtpRepository
    {
        Task AddAsync(OtpRequest otp);
        Task<OtpRequest?> GetLatestValidOtpAsync(Guid userId, string otpCode);
        Task UpdateAsync(OtpRequest otp);
    }
}
