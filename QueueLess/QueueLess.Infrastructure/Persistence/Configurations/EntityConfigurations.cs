using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueLess.Domain.Entities;

namespace QueueLess.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.MobileNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(u => u.MobileNumber)
                .IsUnique();

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(20);
        }
    }

    public class BusinessCategoryConfiguration : IEntityTypeConfiguration<BusinessCategory>
    {
        public void Configure(EntityTypeBuilder<BusinessCategory> builder)
        {
            builder.ToTable("BusinessCategories");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.IconUrl)
                .HasMaxLength(500);

            builder.Property(c => c.Description)
                .HasMaxLength(300);
        }
    }

    public class BusinessConfiguration : IEntityTypeConfiguration<Business>
    {
        public void Configure(EntityTypeBuilder<Business> builder)
        {
            builder.ToTable("Businesses");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.Description)
                .HasMaxLength(500);

            builder.Property(b => b.Address)
                .HasMaxLength(200);

            // Geographic coordinates used for location-based recommendations.
            builder.Property(b => b.Latitude)
                .HasPrecision(9, 6);

            builder.Property(b => b.Longitude)
                .HasPrecision(9, 6);

            // Used to rank businesses in Popular Services.
            builder.Property(b => b.PopularityScore)
                .HasDefaultValue(0);

            builder.Property(b => b.ImageUrl)
                .HasMaxLength(500);

            builder.HasOne(b => b.Category)
                .WithMany(c => c.Businesses)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.ToTable("Services");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Description)
                .HasMaxLength(500);

            builder.HasOne(s => s.Business)
                .WithMany(b => b.Services)
                .HasForeignKey(s => s.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.AssignedStaff)
                .WithOne(st => st.Service)
                .HasForeignKey<Service>(s => s.AssignedStaffId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class WorkingHoursConfiguration : IEntityTypeConfiguration<WorkingHours>
    {
        public void Configure(EntityTypeBuilder<WorkingHours> builder)
        {
            builder.ToTable("WorkingHours");
            builder.HasKey(w => w.Id);

            builder.HasOne(w => w.Service)
                .WithMany(s => s.WorkingHours)
                .HasForeignKey(w => w.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class StaffConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder)
        {
            builder.ToTable("Staff");
            builder.HasKey(st => st.Id);

            builder.HasOne(st => st.User)
                .WithOne()
                .HasForeignKey<Staff>(st => st.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class PlatformAdminConfiguration : IEntityTypeConfiguration<PlatformAdmin>
    {
        public void Configure(EntityTypeBuilder<PlatformAdmin> builder)
        {
            builder.ToTable("PlatformAdmins");
            builder.HasKey(pa => pa.Id);

            builder.HasOne(pa => pa.User)
                .WithOne()
                .HasForeignKey<PlatformAdmin>(pa => pa.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("Tickets");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.HasOne(t => t.Service)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Customer)
                .WithMany()
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Constraint: A customer can hold at most one active ticket
            // (Status = Waiting or Serving).
            builder.HasIndex(t => t.CustomerId)
                .IsUnique()
                .HasFilter("[Status] IN ('Waiting', 'Serving')");
        }
    }

    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(1000);

            builder.HasOne(n => n.Ticket)
                .WithMany()
                .HasForeignKey(n => n.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}