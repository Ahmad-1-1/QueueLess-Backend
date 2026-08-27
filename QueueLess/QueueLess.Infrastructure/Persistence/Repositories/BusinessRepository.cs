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
                .ToListAsync();
        }

        public async Task<List<Business>> GetRecommendedBusinessesAsync(string? categoryName = null, string? search = null, string? location = null)
        {
            var query = _context.Businesses
                .Include(b => b.Category)
                .Include(b => b.Services)
                .Where(b => b.IsActive)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(categoryName) && !string.Equals(categoryName, "All", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(b => b.Category != null && b.Category.Name.ToLower() == categoryName.Trim().ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(b => b.Name.ToLower().Contains(term) ||
                                         b.Description.ToLower().Contains(term) ||
                                         (b.Category != null && b.Category.Name.ToLower().Contains(term)) ||
                                         b.Services.Any(s => s.Name.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                var loc = location.Trim().ToLower();
                query = query.Where(b => b.Location.ToLower().Contains(loc) || b.Address.ToLower().Contains(loc));
            }

            return await query.ToListAsync();
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
