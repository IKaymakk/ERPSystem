using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces
{
    public interface IFileUploadService
    {
        Task<string> UploadProductImageAsync(Stream fileStream, string fileName, string contentType);
        Task<bool> DeleteFileAsync(string filePath);
        Task<bool> FileExistsAsync(string filePath);
        Task<byte[]> GetFileAsync(string filePath);
        string GetFileUrl(string filePath);
        bool IsValidImageFile(string fileName, string contentType);
        Task<string> SaveBase64ImageAsync(string base64String, string fileName);
    }
}
