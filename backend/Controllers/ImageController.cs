using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Walks.API.Models.DTOs;
using Walks.API.Models.Entities;
using Walks.API.Repositories;

namespace Walks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;

        public ImageController(IImageRepository imageRepository, IMapper mapper)
        {
            _imageRepository = imageRepository;
            _mapper = mapper;
        }

        // POST: /api/Image/Upload
        [HttpPost("Upload")]
        [Authorize(Roles = "Writer")]
        public async Task<ActionResult<ImageDto>> Upload([FromForm] ImageUploadRequestDto request)
        {
            ValidateFileUpload(request);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var imageDomainModel = _mapper.Map<Image>(request);
            
            var image = await _imageRepository.UploadAsync(imageDomainModel);

            var imageResponse = _mapper.Map<ImageDto>(image);

            return Ok(imageResponse);
        }

        private void ValidateFileUpload(ImageUploadRequestDto request)
        {
            if (request.File is null)
            {
                ModelState.AddModelError(nameof(request.File), "File is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(request.FileName))
            {
                ModelState.AddModelError(nameof(request.FileName), "FileName is required.");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError(nameof(request.File), "Unsupported file extension. Allowed: .jpg, .jpeg, .png");
            }

            if (request.File.Length > 10485760)
            {
                ModelState.AddModelError(nameof(request.File), "File size more than 10MB, please upload a smaller file.");
            }
        }
    }
}
