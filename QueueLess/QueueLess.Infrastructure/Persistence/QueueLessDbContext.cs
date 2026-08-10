using Microsoft.EntityFrameworkCore;
using QueueLess.Domain.Entities;

namespace QueueLess.Infrastructure.Persistence
{
    public class QueueLessDbContext : DbContext
    {
        public QueueLessDbContext(DbContextOptions<QueueLessDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<BusinessCategory> BusinessCategories => Set<BusinessCategory>();
        public DbSet<Business> Businesses => Set<Business>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<WorkingHours> WorkingHours => Set<WorkingHours>();
        public DbSet<Staff> Staff => Set<Staff>();
        public DbSet<PlatformAdmin> PlatformAdmins => Set<PlatformAdmin>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(QueueLessDbContext).Assembly);
        }
    }
}
