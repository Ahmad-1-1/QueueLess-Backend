namespace QueueLess.Infrastructure.Services
{
    public class PictureOptions
    {
        public const string SectionName = "Pictures";

        public string DefaultBusinessImage { get; set; } = string.Empty;

        public string DefaultCategoryIcon { get; set; } = string.Empty;

        public string DefaultPopularServiceImage { get; set; } = string.Empty;
    }
}