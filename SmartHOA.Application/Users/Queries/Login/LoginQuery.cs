using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartHOA.Application.Common.Interfaces;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace SmartHOA.Application.Users.Queries.Login;

public record LoginQuery(string Email, string Password) : IRequest<string>;

public class LoginQueryHandler : IRequestHandler<LoginQuery, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    
    public LoginQueryHandler(IApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<string> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.PasswordHash == request.Password, cancellationToken);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Грешен имейл или парола.");
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("FirstName", user.FirstName),
            new Claim(ClaimTypes.Role, user.UserType.ToString()),
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiryMinutes"]!)),
            signingCredentials: creds
            );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}