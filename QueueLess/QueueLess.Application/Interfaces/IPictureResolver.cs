namespace QueueLess.Application.Interfaces
{
    public interface IPictureResolver
    {
        string ResolveBusinessImage(string? imageUrl);

        string ResolveCategoryIcon(string? iconUrl);

        string ResolvePopularServiceImage(string? imageUrl);
    }
}