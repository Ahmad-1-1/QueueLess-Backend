using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QueueLess.Domain.Entities;

namespace QueueLess.Application.Interfaces
{
    public interface IBusinessRepository
    {
        Task<List<BusinessCategory>> GetCategoriesAsync();

        Task<List<Business>> GetRecommendedBusinessesAsync(
            double? latitude = null,
            double? longitude = null,
            string? categoryName = null,
            string? search = null);

        Task<List<Business>> GetPopularBusinessesAsync(int topN);

        Task<Business?> GetByIdAsync(Guid id);

        Task AddCategoryAsync(BusinessCategory category);

        Task AddBusinessAsync(Business business);
    }
}