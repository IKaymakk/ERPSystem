using ERPSystem.Application.Interfaces;
using ERPSystem.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileUploadService> _logger;
        private readonly string _uploadPath;
        private readonly string _baseUrl;
        private readonly long _maxFileSize;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        private readonly string[] _allowedMimeTypes = { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/bmp", "image/webp" };

        public FileUploadService(IConfiguration configuration, ILogger<FileUploadService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _uploadPath = _configuration.GetValue<string>("FileUpload:ProductImagesPath") ?? "wwwroot/uploads/products";
            _baseUrl = _configuration.GetValue<string>("FileUpload:BaseUrl") ?? "/uploads/products";
            _maxFileSize = _configuration.GetValue<long>("FileUpload:MaxFileSize"); // 5MB

            // Upload klasörünü oluştur
            EnsureUploadDirectoryExists();
        }

        public async Task<string> UploadProductImageAsync(Stream fileStream, string fileName, string contentType)
        {
            try
            {
                // Validasyonlar
                if (fileStream == null || fileStream.Length == 0)
                    throw new BusinessException("Dosya Boş");

                if (fileStream.Length > _maxFileSize)
                    throw new BusinessException($"Desteklenen Maksimum Dosya Boyutu : {_maxFileSize / 1024 / 1024}MB");

                if (!IsValidImageFile(fileName, contentType))
                    throw new BusinessException("Geçersiz Dosya Tipi. Sadece Resim Dosyaları Kabul Edilir.");

                // Unique dosya adı oluştur
                var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var fullPath = Path.Combine(_uploadPath, uniqueFileName);

                // Dosyayı kaydet
                using (var fileStreamOutput = new FileStream(fullPath, FileMode.Create))
                {
                    await fileStream.CopyToAsync(fileStreamOutput);
                }

                _logger.LogInformation($"Product image uploaded successfully: {uniqueFileName}");

                // Relative path döndür
                return Path.Combine(_baseUrl, uniqueFileName).Replace("\\", "/");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading product image: {fileName}");
                throw new BusinessException($"Error uploading file: {ex.Message}");
            }
        }

        public async Task<string> SaveBase64ImageAsync(string base64String, string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(base64String))
                    throw new BusinessException("Base64 string is empty");

                // Base64 string'i decode et
                var base64Data = base64String;
                if (base64Data.Contains(","))
                {
                    base64Data = base64Data.Split(',')[1]; // data:image/jpeg;base64, kısmını çıkar
                }

                var imageBytes = Convert.FromBase64String(base64Data);

                if (imageBytes.Length > _maxFileSize)
                    throw new BusinessException($"File size exceeds maximum limit of {_maxFileSize / 1024 / 1024}MB");

                // Memory stream oluştur
                using (var memoryStream = new MemoryStream(imageBytes))
                {
                    return await UploadProductImageAsync(memoryStream, fileName, "image/jpeg");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving base64 image: {fileName}");
                throw new BusinessException($"Error saving base64 image: {ex.Message}");
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return false;

                // Relative path'i full path'e çevir
                var fileName = Path.GetFileName(filePath);
                var fullPath = Path.Combine(_uploadPath, fileName);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation($"File deleted successfully: {fileName}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting file: {filePath}");
                return false;
            }
        }

        public async Task<bool> FileExistsAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return false;

                var fileName = Path.GetFileName(filePath);
                var fullPath = Path.Combine(_uploadPath, fileName);

                return File.Exists(fullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking file existence: {filePath}");
                return false;
            }
        }

        public async Task<byte[]> GetFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    throw new NotFoundException($"File not found: {filePath}");

                var fileName = Path.GetFileName(filePath);
                var fullPath = Path.Combine(_uploadPath, fileName);

                if (!File.Exists(fullPath))
                    throw new NotFoundException($"File not found: {filePath}");

                return await File.ReadAllBytesAsync(fullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reading file: {filePath}");
                throw new BusinessException($"Error reading file: {ex.Message}");
            }
        }

        public string GetFileUrl(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            // Eğer zaten tam URL ise, olduğu gibi döndür
            if (filePath.StartsWith("http"))
                return filePath;

            // Relative path ise, base URL ile birleştir
            return filePath.Replace("\\", "/");
        }

        public bool IsValidImageFile(string fileName, string contentType)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(contentType))
                return false;

            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return _allowedExtensions.Contains(extension) && _allowedMimeTypes.Contains(contentType.ToLowerInvariant());
        }

        private void EnsureUploadDirectoryExists()
        {
            try
            {
                if (!Directory.Exists(_uploadPath))
                {
                    Directory.CreateDirectory(_uploadPath);
                    _logger.LogInformation($"Upload directory created: {_uploadPath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating upload directory: {_uploadPath}");
                throw new BusinessException($"Error creating upload directory: {ex.Message}");
            }
        }
    }
}
