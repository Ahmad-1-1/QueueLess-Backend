using System.Threading;
using System.Threading.Tasks;
using QueueLess.Application.Interfaces;

namespace QueueLess.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly QueueLessDbContext _context;

        public UnitOfWork(QueueLessDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
