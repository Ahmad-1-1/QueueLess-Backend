using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueLess.Domain.Entities
{
    public class OtpRequest
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string OtpCode { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
