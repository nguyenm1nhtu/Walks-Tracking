namespace Walks.API.Models.Entities
{
    public class Region
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? RegionImageUrl { get; set; }
    }
}
