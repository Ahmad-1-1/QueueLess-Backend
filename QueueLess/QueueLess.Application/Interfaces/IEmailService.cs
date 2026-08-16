using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueLess.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string toEmail, string fullName, string otpCode);
    }
}
