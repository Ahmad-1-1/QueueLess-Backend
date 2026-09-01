using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QueueLess.Application.Interfaces;
using QueueLess.Domain.Entities;

namespace QueueLess.Infrastructure.Persistence.Repositories
{
    public class BusinessRepository : IBusinessRepository
    {
        private readonly QueueLessDbContext _context;

        public BusinessRepository(QueueLessDbContext context)
        {
            _context = context;
        }

        public async Task<List<BusinessCategory>> GetCategoriesAsync()
        {
            return await _context.BusinessCategories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<List<Business>> GetRecommendedBusinessesAsync(
            double? latitude = null,
            double? longitude = null,
            string? categoryName = null,
            string? search = null)
        {
            var query = _context.Businesses
                .Include(b => b.Category)
                .Include(b => b.Services)
                .Where(b => b.IsActive)
                .AsNoTracking()
                .AsQueryable();

            // Category filter
            if (!string.IsNullOrWhiteSpace(categoryName) &&
                !string.Equals(categoryName, "All", StringComparison.OrdinalIgnoreCase))
            {
                var category = categoryName.Trim().ToLower();

                query = query.Where(b =>
                    b.Category != null &&
                    b.Category.Name.ToLower() == category);
            }

            // Search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();

                query = query.Where(b =>
                    b.Name.ToLower().Contains(term) ||
                    b.Description.ToLower().Contains(term) ||
                    (b.Category != null &&
                     b.Category.Name.ToLower().Contains(term)) ||
                    b.Services.Any(s =>
                        s.Name.ToLower().Contains(term)));
            }

            // When GPS coordinates are not provided,
            // return active businesses ordered by rating.
            if (!latitude.HasValue || !longitude.HasValue)
            {
                return await query
                    .OrderByDescending(b => b.Rating)
                    .ThenByDescending(b => b.PopularityScore)
                    .ThenBy(b => b.Name)
                    .ToListAsync();
            }

            // Get businesses that have coordinates.
            // Distance calculation is handled in the application layer.
            query = query.Where(b =>
                b.Latitude.HasValue &&
                b.Longitude.HasValue);

            return await query
                .OrderByDescending(b => b.Rating)
                .ThenByDescending(b => b.PopularityScore)
                .ThenBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<List<Business>> GetPopularBusinessesAsync(int topN)
        {
            if (topN <= 0)
            {
                return new List<Business>();
            }

            return await _context.Businesses
                .Include(b => b.Category)
                .Include(b => b.Services)
                .Where(b => b.IsActive)
                .AsNoTracking()
                .OrderByDescending(b => b.PopularityScore)
                .ThenByDescending(b => b.Rating)
                .ThenBy(b => b.Name)
                .Take(topN)
                .ToListAsync();
        }

        public async Task<Business?> GetByIdAsync(Guid id)
        {
            return await _context.Businesses
                .Include(b => b.Category)
                .Include(b => b.Services)
                .ThenInclude(s => s.WorkingHours)
                .FirstOrDefaultAsync(b => b.Id == id && b.IsActive);
        }

        public async Task AddCategoryAsync(BusinessCategory category)
        {
            await _context.BusinessCategories.AddAsync(category);
        }

        public async Task AddBusinessAsync(Business business)
        {
            await _context.Businesses.AddAsync(business);
        }
    }
}