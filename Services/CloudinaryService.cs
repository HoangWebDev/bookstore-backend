using BookStore.Request;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace BookStore.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryService(IOptions<CloudinaryRequest> config)
        {
            var account = new Account(
                config.Value.CloudName, 
                config.Value.ApiKey, 
                config.Value.ApiSecret
                );

            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or null.");
            }

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "Book_Store", // Thêm folder vào đường dẫn lưu
                Transformation = new Transformation()
                                .Quality(80) //Giảm dung lương ảnh
                                .FetchFormat("jpg") // Nén ảnh
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            // Kiểm tra lỗi từ Cloudinary
            if (uploadResult.Error != null)
            {
                throw new Exception($"Cloudinary error: {uploadResult.Error.Message}");
            }

            // Kiểm tra nếu SecureUrl có giá trị thì trả về
            return uploadResult.SecureUrl?.ToString() ?? throw new Exception("Failed to upload image to Cloudinary.");
        }

    }
}
