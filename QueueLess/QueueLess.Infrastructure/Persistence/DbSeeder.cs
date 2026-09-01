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
            // ============================================================
            // 1. Seed Business Categories
            // ============================================================

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

            // ============================================================
            // 2. Get Categories
            // ============================================================

            var bankCategory =
                await context.BusinessCategories
                    .FirstOrDefaultAsync(c => c.Name == "Bank");

            var hospitalCategory =
                await context.BusinessCategories
                    .FirstOrDefaultAsync(c => c.Name == "Hospital");

            var clinicCategory =
                await context.BusinessCategories
                    .FirstOrDefaultAsync(c => c.Name == "Clinic");

            var telecomCategory =
                await context.BusinessCategories
                    .FirstOrDefaultAsync(c => c.Name == "Telecom");

            var governmentCategory =
                await context.BusinessCategories
                    .FirstOrDefaultAsync(c => c.Name == "Government");

            if (bankCategory == null ||
                hospitalCategory == null ||
                clinicCategory == null ||
                telecomCategory == null ||
                governmentCategory == null)
            {
                throw new InvalidOperationException(
                    "One or more required business categories were not found.");
            }

            // ============================================================
            // 3. Define Seed Businesses
            // ============================================================

            var seedBusinesses = new List<Business>
            {
                new Business
                {
                    Name = "City General Hospital",
                    Description =
                        "Emergency care, specialist consultation & surgery",
                    Address =
                        "123 El-Giesh Street, Tanta",

                    Latitude = 30.7865,
                    Longitude = 31.0004,

                    Rating = 4.6,
                    PopularityScore = 95,
                    IsOpen = true,

                    ImageUrl =
                        "https://images.unsplash.com/photo-1587351021759-3e566b6af7cc?w=600",

                    CategoryId = hospitalCategory.Id
                },

                new Business
                {
                    Name = "Metro Bank – CBD Branch",
                    Description =
                        "Accounts, loans, customer service & foreign exchange",
                    Address =
                        "45 El-Bahr Street, Tanta",

                    Latitude = 30.7902,
                    Longitude = 30.9988,

                    Rating = 4.5,
                    PopularityScore = 88,
                    IsOpen = true,

                    ImageUrl =
                        "https://images.unsplash.com/photo-1541354329998-f4d9a9f9297f?w=600",

                    CategoryId = bankCategory.Id
                },

                new Business
                {
                    Name = "National Health Clinic",
                    Description =
                        "General check-ups, vaccinations & dental service",
                    Address =
                        "78 Saeed Street, Tanta",

                    Latitude = 30.7815,
                    Longitude = 31.0045,

                    Rating = 4.8,
                    PopularityScore = 92,
                    IsOpen = true,

                    ImageUrl =
                        "https://images.unsplash.com/photo-1629909613654-28e377c37b09?w=600",

                    CategoryId = clinicCategory.Id
                },

                new Business
                {
                    Name = "Banque Misr – Tanta Branch",
                    Description =
                        "Personal banking, corporate services & ATMs",
                    Address =
                        "10 Hassan Radwan St, Tanta",

                    Latitude = 30.7930,
                    Longitude = 31.0055,

                    Rating = 4.7,
                    PopularityScore = 90,
                    IsOpen = true,

                    ImageUrl =
                        "https://images.unsplash.com/photo-1501167786227-4cba60f6d58f?w=600",

                    CategoryId = bankCategory.Id
                },

                new Business
                {
                    Name = "Vodafone Service Center",
                    Description =
                        "SIM activation, home internet & bill payments",
                    Address =
                        "15 Al-Nassr Square, Tanta",

                    Latitude = 30.7840,
                    Longitude = 30.9945,

                    Rating = 4.4,
                    PopularityScore = 78,
                    IsOpen = true,

                    ImageUrl =
                        "https://images.unsplash.com/photo-1556740758-90de374c12ad?w=600",

                    CategoryId = telecomCategory.Id
                },

                new Business
                {
                    Name = "Civil Registry Office",
                    Description =
                        "National ID issuance, birth certificates & official records",
                    Address =
                        "30 Government Complex, Tanta",

                    Latitude = 30.7980,
                    Longitude = 31.0090,

                    Rating = 4.2,
                    PopularityScore = 65,
                    IsOpen = true,

                    ImageUrl =
                        "https://images.unsplash.com/photo-1526304640581-d334cdbbf45e?w=600",

                    CategoryId = governmentCategory.Id
                },

                new Business
                {
                    Name = "Dental Care Specialized Center",
                    Description =
                        "Teeth whitening, implants, orthodontics & pediatric care",
                    Address =
                        "90 Moheb Street, Tanta",

                    Latitude = 30.7775,
                    Longitude = 30.9970,

                    Rating = 4.9,
                    PopularityScore = 97,
                    IsOpen = true,

                    ImageUrl =
                        "https://images.unsplash.com/photo-1588776814546-1ffcf47267a5?w=600",

                    CategoryId = clinicCategory.Id
                }
            };

            // ============================================================
            // 4. Insert OR Update Businesses
            // ============================================================

            foreach (var seedBusiness in seedBusinesses)
            {
                var existingBusiness =
                    await context.Businesses
                        .FirstOrDefaultAsync(b =>
                            b.Name == seedBusiness.Name);

                if (existingBusiness == null)
                {
                    seedBusiness.Id = Guid.NewGuid();

                    await context.Businesses.AddAsync(seedBusiness);
                }
                else
                {
                    // Update only the seed-controlled properties.
                    // Keep the existing business ID.
                    existingBusiness.Description =
                        seedBusiness.Description;

                    existingBusiness.Address =
                        seedBusiness.Address;

                    existingBusiness.Latitude =
                        seedBusiness.Latitude;

                    existingBusiness.Longitude =
                        seedBusiness.Longitude;

                    existingBusiness.Rating =
                        seedBusiness.Rating;

                    existingBusiness.PopularityScore =
                        seedBusiness.PopularityScore;

                    existingBusiness.IsOpen =
                        seedBusiness.IsOpen;

                    existingBusiness.ImageUrl =
                        seedBusiness.ImageUrl;

                    existingBusiness.CategoryId =
                        seedBusiness.CategoryId;

                    existingBusiness.IsActive = true;
                }
            }

            await context.SaveChangesAsync();

            // ============================================================
            // 5. Get Businesses
            // ============================================================

            var businesses =
                await context.Businesses
                    .Where(b =>
                        b.Name == "City General Hospital" ||
                        b.Name == "Metro Bank – CBD Branch" ||
                        b.Name == "National Health Clinic" ||
                        b.Name == "Banque Misr – Tanta Branch" ||
                        b.Name == "Vodafone Service Center" ||
                        b.Name == "Civil Registry Office" ||
                        b.Name == "Dental Care Specialized Center")
                    .ToDictionaryAsync(
                        b => b.Name,
                        b => b.Id);

            // ============================================================
            // 6. Seed Services
            // ============================================================

            var seedServices = new List<Service>();

            if (businesses.TryGetValue(
                "City General Hospital",
                out var cityHospitalId))
            {
                seedServices.Add(
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        BusinessId = cityHospitalId,
                        Name = "Emergency Care",
                        Description =
                            "Urgent medical triage & care",
                        AvgServiceTimeMinutes = 15
                    });

                seedServices.Add(
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        BusinessId = cityHospitalId,
                        Name = "Specialist Consultation",
                        Description =
                            "Outpatient doctor appointments",
                        AvgServiceTimeMinutes = 20
                    });
            }

            if (businesses.TryGetValue(
                "Metro Bank – CBD Branch",
                out var metroBankId))
            {
                seedServices.Add(
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        BusinessId = metroBankId,
                        Name = "Teller & Cash Deposit",
                        Description =
                            "Cash transactions, deposits & withdrawals",
                        AvgServiceTimeMinutes = 10
                    });

                seedServices.Add(
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        BusinessId = metroBankId,
                        Name = "Customer Account Services",
                        Description =
                            "New accounts, credit cards & loans",
                        AvgServiceTimeMinutes = 25
                    });
            }

            if (businesses.TryGetValue(
                "National Health Clinic",
                out var clinicId))
            {
                seedServices.Add(
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        BusinessId = clinicId,
                        Name = "General Health Check-up",
                        Description =
                            "Routine body check-up & blood tests",
                        AvgServiceTimeMinutes = 15
                    });
            }

            if (businesses.TryGetValue(
                "Banque Misr – Tanta Branch",
                out var banqueMisrId))
            {
                seedServices.Add(
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        BusinessId = banqueMisrId,
                        Name = "Teller Services",
                        Description =
                            "Cash deposit & transfer",
                        AvgServiceTimeMinutes = 8
                    });
            }

            if (businesses.TryGetValue(
                "Vodafone Service Center",
                out var vodafoneId))
            {
                seedServices.Add(
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        BusinessId = vodafoneId,
                        Name = "SIM Swap & Activation",
                        Description =
                            "New SIMs, 4G/5G upgrades & line issues",
                        AvgServiceTimeMinutes = 12
                    });
            }

            if (businesses.TryGetValue(
                "Civil Registry Office",
                out var civilRegistryId))
            {
                seedServices.Add(
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        BusinessId = civilRegistryId,
                        Name = "National ID Renewal",
                        Description =
                            "Civil status & national card renewal",
                        AvgServiceTimeMinutes = 30
                    });
            }

            if (businesses.TryGetValue(
                "Dental Care Specialized Center",
                out var dentalId))
            {
                seedServices.Add(
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        BusinessId = dentalId,
                        Name = "Dental Consultation & Cleaning",
                        Description =
                            "Teeth checkup & dental hygienist",
                        AvgServiceTimeMinutes = 20
                    });
            }

            // ============================================================
            // 7. Insert Services Only If They Don't Already Exist
            // ============================================================

            foreach (var seedService in seedServices)
            {
                var exists =
                    await context.Services.AnyAsync(s =>
                        s.BusinessId == seedService.BusinessId &&
                        s.Name == seedService.Name);

                if (!exists)
                {
                    await context.Services.AddAsync(seedService);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}