using System;
using QueueLess.Domain.Enums;

namespace QueueLess.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string MobileNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
