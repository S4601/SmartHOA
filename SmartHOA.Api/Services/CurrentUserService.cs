using System.Security.Claims;
using SmartHOA.Application.Common.Interfaces;

namespace SmartHOA.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return null;

            var idClaim = user.FindFirstValue(ClaimTypes.NameIdentifier) 
                          ?? user.FindFirstValue("sub");

            if (Guid.TryParse(idClaim, out var userId))
            {
                return userId;
            }

            return null;
        }
    }

    public string? Role
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return null;

            return user.FindFirstValue(ClaimTypes.Role) ?? user.FindFirstValue("role");
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
