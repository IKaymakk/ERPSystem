using ERPSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERPSystem.Application.Interfaces;
using ERPSystem.Core.Exceptions;
using BC = BCrypt.Net.BCrypt;

namespace ERPSystem.Application.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly int _workFactor;

        public PasswordService(int workFactor = 12)
        {
            _workFactor = workFactor;
        }

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new BusinessException("Password cannot be null or empty");

            return BC.HashPassword(password, _workFactor);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            try
            {
                return BC.Verify(password, hashedPassword);
            }
            catch
            {
                return false; // Hash bozuksa false döner
            }
        }

        /// <summary>
        /// Şifrenin hash'li olup olmadığını kontrol eder ve gerekirse hashler
        /// </summary>
        public string EnsurePasswordIsHashed(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new BusinessException("Password cannot be null or empty");

            if (IsValidBCryptHash(password))
                return password;

            return HashPassword(password);
        }

        /// <summary>
        /// String'in geçerli BCrypt hash formatında olup olmadığını kontrol eder
        /// </summary>
        public bool IsValidBCryptHash(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                return false;

            if (hash.Length != 60)
                return false;

            if (!hash.StartsWith("$2a$") && !hash.StartsWith("$2b$") && !hash.StartsWith("$2y$"))
                return false;

            try
            {
                BC.Verify("test", hash);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
