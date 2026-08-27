using System.Threading.Tasks;
using QueueLess.Application.DTOs;

namespace QueueLess.Application.Interfaces
{
    public interface IHomeService
    {
        Task<HomePageDataResponse> GetHomePageDataAsync(string? category = null, string? search = null, string? location = null);
    }
}
