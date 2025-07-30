using AutoMapper;
using ERPSystem.Application.Interfaces;
using ERPSystem.Core.DTOs.Auth;
using ERPSystem.Core.Entities;
using ERPSystem.Core.Exceptions;
using ERPSystem.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly int _refreshTokenExpirationDays;

        public AuthService(
            IUserRepository userRepository,
            IPasswordService passwordService,
            IJwtService jwtService,
            IMapper mapper,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
            _mapper = mapper;
            _configuration = configuration;
            _refreshTokenExpirationDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "30");
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            User? user = null;

            if (loginDto.EmailOrUsername.Contains("@"))
            {
                user = await _userRepository.GetByEmailAsync(loginDto.EmailOrUsername);
            }
            else
            {
                user = await _userRepository.GetByUsernameAsync(loginDto.EmailOrUsername);
            }

            // 2. Kullanıcı kontrolü
            if (user == null)
                throw new BusinessException("Kullanıcı adı/e-posta veya şifre hatalı.");

            if (!user.IsActive)
                throw new BusinessException("Hesabınız pasif durumda. Lütfen yönetici ile iletişime geçin.");

            // 3. Şifre kontrolü
            if (!_passwordService.VerifyPassword(loginDto.Password, user.PasswordHash))
                throw new BusinessException("Kullanıcı adı/e-posta veya şifre hatalı.");

            // 4. JWT claims hazırla
            var claims = new JwtClaimsDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                RoleName = user.Role?.Name ?? "User",
                RoleId = user.RoleId
            };

            // 5. Token'ları oluştur
            var accessToken = _jwtService.GenerateAccessToken(claims);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60"));

            // 6. Refresh token'ı veritabanına kaydet
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);
            user.LastLoginDate = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            // 7. Response hazırla
            var userDto = _mapper.Map<UserDto>(user);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                User = userDto
            };
        }

        public async Task<TokenDto> RefreshTokenAsync(string refreshToken)
        {
            // 1. Refresh token ile kullanıcıyı bul
            var user = await _userRepository.GetByRefreshTokenForUpdateAsync(refreshToken);

            if (user == null)
                throw new BusinessException("Geçersiz refresh token.");

            if (!user.IsActive)
                throw new BusinessException("Hesabınız pasif durumda.");

            // 2. Refresh token süresini kontrol et
            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new BusinessException("Refresh token süresi dolmuş. Lütfen tekrar giriş yapın.");

            // 3. Yeni JWT claims hazırla
            var claims = new JwtClaimsDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                RoleName = user.Role?.Name ?? "User",
                RoleId = user.RoleId
            };

            // 4. Yeni token'ları oluştur
            var newAccessToken = _jwtService.GenerateAccessToken(claims);
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60"));

            // 5. Yeni refresh token'ı kaydet
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);

            await _userRepository.UpdateAsync(user);

            return new TokenDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = expiresAt
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return; // Silent logout

            // Refresh token'ı temizle
            var user = await _userRepository.GetByRefreshTokenForUpdateAsync(refreshToken);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task<UserDto> GetCurrentUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.IsActive)
                throw new BusinessException("Kullanıcı bulunamadı.");

            return _mapper.Map<UserDto>(user);
        }
    }
}
