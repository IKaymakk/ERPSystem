using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ERPSystem.Core.Entities;

namespace ERPSystem.Core.Interfaces;
public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    bool ValidateToken(string token);
}
