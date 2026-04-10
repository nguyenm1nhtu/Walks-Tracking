using Walks.API.Models.Entities;

namespace Walks.API.Repositories
{
    public interface IImageRepository
    {
        Task<Image> UploadAsync(Image image);
    }
}
