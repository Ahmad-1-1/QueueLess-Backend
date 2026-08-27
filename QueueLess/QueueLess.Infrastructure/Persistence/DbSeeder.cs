using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QueueLess.Domain.Entities;

namespace QueueLess.Infrastructure.Persistence
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(QueueLessDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            // 1. Seed Business Categories if none exist
            if (!await context.BusinessCategories.AnyAsync())
            {
                var categories = new List<BusinessCategory>
                {
                    new BusinessCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = "Bank",
                        Description = "Financial Transactions, Cash Deposits & Loans",
                        IconUrl = "https://cdn-icons-png.flaticon.com/512/2830/2830284.png"
                    },
                    new BusinessCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = "Hospital",
                        Description = "Emergency Care, Surgery & Specialist Clinics",
                        IconUrl = "https://cdn-icons-png.flaticon.com/512/3063/3063822.png"
                    },
                    new BusinessCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = "Clinic",
                        Description = "General Check-ups, Dental & Pediatrics",
                        IconUrl = "https://cdn-icons-png.flaticon.com/512/2966/2966327.png"
                    },
                    new BusinessCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = "Government",
                        Description = "National ID, Passports & Civil Registry Services",
                        IconUrl = "https://cdn-icons-png.flaticon.com/512/3135/3135715.png"
                    },
                    new BusinessCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = "Telecom",
                        Description = "SIM Cards, Fiber Internet & Bill Payments",
                        IconUrl = "https://cdn-icons-png.flaticon.com/512/1055/1055687.png"
                    },
                    new BusinessCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = "Customer Service",
                        Description = "Technical Support, Repairs & Inquiries",
                        IconUrl = "https://cdn-icons-png.flaticon.com/512/1067/1067566.png"
                    }
                };

                await context.BusinessCategories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // 2. Seed Businesses if none exist
            if (!await context.Businesses.AnyAsync())
            {
                var bankCategory = await context.BusinessCategories.FirstOrDefaultAsync(c => c.Name == "Bank");
                var hospitalCategory = await context.BusinessCategories.FirstOrDefaultAsync(c => c.Name == "Hospital");
                var clinicCategory = await context.BusinessCategories.FirstOrDefaultAsync(c => c.Name == "Clinic");
                var telecomCategory = await context.BusinessCategories.FirstOrDefaultAsync(c => c.Name == "Telecom");
                var govCategory = await context.BusinessCategories.FirstOrDefaultAsync(c => c.Name == "Government");
                var csCategory = await context.BusinessCategories.FirstOrDefaultAsync(c => c.Name == "Customer Service");

                var b1 = new Business
                {
                    Id = Guid.NewGuid(),
                    Name = "City General Hospital",
                    Description = "Emergency care, specialist consultation & surgery",
                    Address = "123 El-Giesh Street, Tanta",
                    Location = "Downtown",
                    Rating = 4.6,
                    IsOpen = true,
                    Tag = "Popular",
                    ImageUrl = "https://images.unsplash.com/photo-1587351021759-3e566b6af7cc?w=600",
                    CategoryId = hospitalCategory?.Id ?? Guid.NewGuid()
                };

                var b2 = new Business
                {
                    Id = Guid.NewGuid(),
                    Name = "Metro Bank – CBD Branch",
                    Description = "Accounts, loans, customer service & foreign exchange",
                    Address = "45 El-Bahr Street, Tanta",
                    Location = "Central District",
                    Rating = 4.5,
                    IsOpen = true,
                    Tag = "Popular",
                    ImageUrl = "https://images.unsplash.com/photo-1541354329998-f4d9a9f9297f?w=600",
                    CategoryId = bankCategory?.Id ?? Guid.NewGuid()
                };

                var b3 = new Business
                {
                    Id = Guid.NewGuid(),
                    Name = "National Health Clinic",
                    Description = "General check-ups, vaccinations & dental service",
                    Address = "78 Saeed Street, Tanta",
                    Location = "Riverside",
                    Rating = 4.8,
                    IsOpen = true,
                    Tag = "Nearby",
                    ImageUrl = "https://images.unsplash.com/photo-1629909613654-28e377c37b09?w=600",
                    CategoryId = clinicCategory?.Id ?? Guid.NewGuid()
                };

                var b4 = new Business
                {
                    Id = Guid.NewGuid(),
                    Name = "Banque Misr – Tanta Branch",
                    Description = "Personal banking, corporate services & ATMs",
                    Address = "10 Hassan Radwan St, Tanta",
                    Location = "Tanta City Center",
                    Rating = 4.7,
                    IsOpen = true,
                    Tag = "Nearby",
                    ImageUrl = "https://images.unsplash.com/photo-1501167786227-4cba60f6d58f?w=600",
                    CategoryId = bankCategory?.Id ?? Guid.NewGuid()
                };

                var b5 = new Business
                {
                    Id = Guid.NewGuid(),
                    Name = "Vodafone Service Center",
                    Description = "SIM activation, home internet & bill payments",
                    Address = "15 Al-Nassr Square, Tanta",
                    Location = "Tanta City Center",
                    Rating = 4.4,
                    IsOpen = true,
                    Tag = "Popular",
                    ImageUrl = "https://images.unsplash.com/photo-1556740758-90de374c12ad?w=600",
                    CategoryId = telecomCategory?.Id ?? Guid.NewGuid()
                };

                var b6 = new Business
                {
                    Id = Guid.NewGuid(),
                    Name = "Civil Registry Office",
                    Description = "National ID issuance, birth certificates & official records",
                    Address = "30 Government Complex, Tanta",
                    Location = "Downtown",
                    Rating = 4.2,
                    IsOpen = true,
                    Tag = "Government",
                    ImageUrl = "https://images.unsplash.com/photo-1526304640581-d334cdbbf45e?w=600",
                    CategoryId = govCategory?.Id ?? Guid.NewGuid()
                };

                var b7 = new Business
                {
                    Id = Guid.NewGuid(),
                    Name = "Dental Care Specialized Center",
                    Description = "Teeth whitening, implants, orthodontics & pediatric care",
                    Address = "90 Moheb Street, Tanta",
                    Location = "Riverside",
                    Rating = 4.9,
                    IsOpen = true,
                    Tag = "Popular",
                    ImageUrl = "https://images.unsplash.com/photo-1588776814546-1ffcf47267a5?w=600",
                    CategoryId = clinicCategory?.Id ?? Guid.NewGuid()
                };

                var businesses = new List<Business> { b1, b2, b3, b4, b5, b6, b7 };
                await context.Businesses.AddRangeAsync(businesses);
                await context.SaveChangesAsync();

                // 3. Seed Services for each Business
                var services = new List<Service>
                {
                    new Service { Id = Guid.NewGuid(), BusinessId = b1.Id, Name = "Emergency Care", Description = "Urgent medical triage & care", AvgServiceTimeMinutes = 15 },
                    new Service { Id = Guid.NewGuid(), BusinessId = b1.Id, Name = "Specialist Consultation", Description = "Outpatient doctor appointments", AvgServiceTimeMinutes = 20 },
                    new Service { Id = Guid.NewGuid(), BusinessId = b2.Id, Name = "Teller & Cash Deposit", Description = "Cash transactions, deposits & withdrawals", AvgServiceTimeMinutes = 10 },
                    new Service { Id = Guid.NewGuid(), BusinessId = b2.Id, Name = "Customer Account Services", Description = "New accounts, credit cards & loans", AvgServiceTimeMinutes = 25 },
                    new Service { Id = Guid.NewGuid(), BusinessId = b3.Id, Name = "General Health Check-up", Description = "Routine body check-up & blood tests", AvgServiceTimeMinutes = 15 },
                    new Service { Id = Guid.NewGuid(), BusinessId = b4.Id, Name = "Teller Services", Description = "Cash deposit & transfer", AvgServiceTimeMinutes = 8 },
                    new Service { Id = Guid.NewGuid(), BusinessId = b5.Id, Name = "SIM Swap & Activation", Description = "New SIMs, 4G/5G upgrades & line issues", AvgServiceTimeMinutes = 12 },
                    new Service { Id = Guid.NewGuid(), BusinessId = b6.Id, Name = "National ID Renewal", Description = "Civil status & national card renewal", AvgServiceTimeMinutes = 30 },
                    new Service { Id = Guid.NewGuid(), BusinessId = b7.Id, Name = "Dental Consultation & Cleaning", Description = "Teeth checkup & dental hygienist", AvgServiceTimeMinutes = 20 }
                };

                await context.Services.AddRangeAsync(services);
                await context.SaveChangesAsync();
            }
        }
    }
}
