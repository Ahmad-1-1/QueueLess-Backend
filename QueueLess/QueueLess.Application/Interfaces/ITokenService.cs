using QueueLess.Domain.Entities;

namespace QueueLess.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
