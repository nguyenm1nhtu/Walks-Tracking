namespace Walks.API.Models.DTOs
{
    public class AddRegionRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? RegionImageUrl { get; set; } = null;
    }
}