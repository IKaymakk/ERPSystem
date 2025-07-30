using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ERPSystem.Core.DTOs.Auth;
using ERPSystem.Core.Entities;

namespace ERPSystem.Application.Interfaces;
public interface IJwtService
{
    string GenerateAccessToken(JwtClaimsDto claims);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
    DateTime GetTokenExpiration(string token);
    bool IsTokenExpired(string token);
}
