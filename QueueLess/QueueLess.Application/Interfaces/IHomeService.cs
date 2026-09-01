using System.Threading.Tasks;
using QueueLess.Application.DTOs;

namespace QueueLess.Application.Interfaces
{
    public interface IHomeService
    {
        Task<HomePageDataResponse> GetHomePageDataAsync(
            double? latitude = null,
            double? longitude = null);
    }
}