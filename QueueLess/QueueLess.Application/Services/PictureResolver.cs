using Microsoft.Extensions.Options;
using QueueLess.Application.Interfaces;

namespace QueueLess.Infrastructure.Services
{
    public class PictureResolver : IPictureResolver
    {
        private readonly PictureOptions _options;

        public PictureResolver(IOptions<PictureOptions> options)
        {
            _options = options.Value;
        }

        public string ResolveBusinessImage(string? imageUrl)
        {
            return Resolve(
                imageUrl,
                _options.DefaultBusinessImage);
        }

        public string ResolveCategoryIcon(string? iconUrl)
        {
            return Resolve(
                iconUrl,
                _options.DefaultCategoryIcon);
        }

        public string ResolvePopularServiceImage(string? imageUrl)
        {
            return Resolve(
                imageUrl,
                _options.DefaultPopularServiceImage);
        }

        private static string Resolve(
            string? imageUrl,
            string fallbackUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return fallbackUrl;
            }

            return imageUrl.Trim();
        }
    }
}