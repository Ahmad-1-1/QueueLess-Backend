using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QueueLess.Domain.Entities;

namespace QueueLess.Application.Interfaces
{
    public interface IBusinessRepository
    {
        Task<List<BusinessCategory>> GetCategoriesAsync();
        Task<List<Business>> GetRecommendedBusinessesAsync(string? categoryName = null, string? search = null, string? location = null);
        Task<Business?> GetByIdAsync(Guid id);
        Task AddCategoryAsync(BusinessCategory category);
        Task AddBusinessAsync(Business business);
    }
}
