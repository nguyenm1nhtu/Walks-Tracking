using Walks.API.Models.Entities;
using Walks.API.Data;

namespace Walks.API.Repositories
{
    public class LocalImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly WalksDbContext _dbContext;
        public LocalImageRepository(
            IWebHostEnvironment webHostEnvironment, 
            IHttpContextAccessor httpContextAccessor,
            WalksDbContext dbContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        public async Task<Image> UploadAsync(Image image)
        {
            ArgumentNullException.ThrowIfNull(image);

            if (image.File is null || image.File.Length == 0)
            {
                throw new ArgumentException("Uploaded file is required.", nameof(image));
            }

            var uploadsFolderPath = Path.Combine(_webHostEnvironment.ContentRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolderPath);

            var fileExtension = image.FileExtension;
            if (string.IsNullOrWhiteSpace(fileExtension))
            {
                fileExtension = Path.GetExtension(image.File.FileName);
            }

            var rawFileName = string.IsNullOrWhiteSpace(image.FileName)
                ? Path.GetFileNameWithoutExtension(image.File.FileName)
                : image.FileName;
            var sanitizedFileName = SanitizeFileName(rawFileName);

            var uniqueFileName = $"{sanitizedFileName}-{Guid.NewGuid():N}{fileExtension.ToLowerInvariant()}";
            var localFilePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            await using var stream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, true);
            await image.File.CopyToAsync(stream);

            image.FileName = sanitizedFileName;
            image.FileExtension = fileExtension.ToLowerInvariant();
            image.FileSizeInBytes = image.File.Length;

            var request = _httpContextAccessor.HttpContext?.Request;
            var relativeFilePath = $"/uploads/{uniqueFileName}";
            image.FilePath = request is null
                ? relativeFilePath
                : $"{request.Scheme}://{request.Host}{request.PathBase}{relativeFilePath}";

            await _dbContext.Images.AddAsync(image);
            await _dbContext.SaveChangesAsync();

            return image;
        }

        private static string SanitizeFileName(string fileName)
        {
            var baseName = Path.GetFileNameWithoutExtension(fileName).Trim();
            if (string.IsNullOrWhiteSpace(baseName))
            {
                return "image";
            }

            foreach (var invalidChar in Path.GetInvalidFileNameChars())
            {
                baseName = baseName.Replace(invalidChar, '-');
            }

            return baseName;
        }
    }
}
