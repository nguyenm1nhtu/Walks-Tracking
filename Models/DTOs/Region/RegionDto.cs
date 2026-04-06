namespace Walks.API.Models.DTOs
{
    public class RegionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? RegionImageUrl { get; set; } = null;
    }
}
