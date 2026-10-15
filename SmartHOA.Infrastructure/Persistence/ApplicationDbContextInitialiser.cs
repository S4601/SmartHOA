using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartHOA.Application.Common.Interfaces;
using SmartHOA.Domain.Entities;
using SmartHOA.Domain.Enums;

namespace SmartHOA.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;

    public ApplicationDbContextInitialiser(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ILogger<ApplicationDbContextInitialiser> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsNpgsql())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Възникна грешка при изпълнение на миграциите на базата данни.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Възникна грешка при началното попълване на базата данни.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        if (!await _context.Users.AnyAsync())
        {
            var adminUser = new User
            {
                FirstName = "Системен",
                LastName = "Администратор",
                Email = "admin@smarthoa.com",
                PhoneNumber = "+359888000000",
                UserType = UserType.Admin,
                PasswordHash = _passwordHasher.HashPassword("Admin123!")
            };

            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Създаден е начален администраторски потребител: admin@smarthoa.com / Admin123!");
        }
    }
}
