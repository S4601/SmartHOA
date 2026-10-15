using SmartHOA.Application.Apartments.Commands;
using SmartHOA.Application.Buildings.Commands.CreateBuilding;
using SmartHOA.Application.Users.Commands.CreateUser;
using SmartHOA.Domain.Entities;
using SmartHOA.Domain.Enums;
using SmartHOA.Infrastructure.Services;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("========================================");
Console.WriteLine("   SmartHOA Automated Test Suite");
Console.WriteLine("========================================");
Console.ResetColor();

int passed = 0;
int failed = 0;

void Assert(bool condition, string testName)
{
    if (condition)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  [PASS] {testName}");
        Console.ResetColor();
        passed++;
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  [FAIL] {testName}");
        Console.ResetColor();
        failed++;
    }
}

// ---------------------------------------------------------
// 1. Password Hasher Tests
// ---------------------------------------------------------
Console.WriteLine("\n[1] Testing PasswordHasher (PBKDF2 SHA-256)...");
var hasher = new PasswordHasher();

var rawPassword = "StrongPassword123!";
var hash = hasher.HashPassword(rawPassword);

Assert(!string.IsNullOrEmpty(hash), "HashPassword should return non-empty hash");
Assert(hash.Split('.').Length == 3, "Hash format should have 3 segments (iterations.salt.hash)");
Assert(hasher.VerifyPassword(rawPassword, hash), "VerifyPassword should return true for correct password");
Assert(!hasher.VerifyPassword("WrongPassword!", hash), "VerifyPassword should return false for incorrect password");
Assert(!hasher.VerifyPassword("", hash), "VerifyPassword should return false for empty password");
Assert(!hasher.VerifyPassword(rawPassword, "tampered.hash.format"), "VerifyPassword should return false for invalid hash format");

// ---------------------------------------------------------
// 2. CreateUserCommandValidator Tests
// ---------------------------------------------------------
Console.WriteLine("\n[2] Testing CreateUserCommandValidator...");
var userValidator = new CreateUserCommandValidator();

var validUserCmd = new CreateUserCommand("Иван", "Иванов", "ivan@example.com", "+359888123456", UserType.Resident, "Secret123!");
var validResult = userValidator.Validate(validUserCmd);
Assert(validResult.IsValid, "Valid CreateUserCommand should pass validation");

var invalidEmailCmd = validUserCmd with { Email = "not-an-email" };
Assert(!userValidator.Validate(invalidEmailCmd).IsValid, "Invalid email format should fail validation");

var shortPassCmd = validUserCmd with { Password = "123" };
Assert(!userValidator.Validate(shortPassCmd).IsValid, "Password shorter than 6 characters should fail validation");

var emptyFirstNameCmd = validUserCmd with { FirstName = "" };
Assert(!userValidator.Validate(emptyFirstNameCmd).IsValid, "Empty FirstName should fail validation");

// ---------------------------------------------------------
// 3. CreateBuildingCommandValidator Tests
// ---------------------------------------------------------
Console.WriteLine("\n[3] Testing CreateBuildingCommandValidator...");
var buildingValidator = new CreateBuildingCommandValidator();

var validBuildingCmd = new CreateBuildingCommand("Блок 105", "ул. Пирин 12", "София", 8, 4, Guid.NewGuid());
Assert(buildingValidator.Validate(validBuildingCmd).IsValid, "Valid CreateBuildingCommand should pass validation");

var zeroFloorCmd = validBuildingCmd with { FloorCount = 0 };
Assert(!buildingValidator.Validate(zeroFloorCmd).IsValid, "FloorCount <= 0 should fail validation");

var emptyCityCmd = validBuildingCmd with { City = "" };
Assert(!buildingValidator.Validate(emptyCityCmd).IsValid, "Empty City should fail validation");

var emptyManagerCmd = validBuildingCmd with { ManagerId = Guid.Empty };
Assert(!buildingValidator.Validate(emptyManagerCmd).IsValid, "Empty ManagerId should fail validation");

// ---------------------------------------------------------
// 4. CreateApartmentCommandValidator Tests
// ---------------------------------------------------------
Console.WriteLine("\n[4] Testing CreateApartmentCommandValidator...");
var aptValidator = new CreateApartmentCommandValidator();

var validAptCmd = new CreateApartmentCommand("12A", 3, Guid.NewGuid());
Assert(aptValidator.Validate(validAptCmd).IsValid, "Valid CreateApartmentCommand should pass validation");

var emptyNumberCmd = validAptCmd with { Number = "" };
Assert(!aptValidator.Validate(emptyNumberCmd).IsValid, "Empty Number should fail validation");

var invalidFloorCmd = validAptCmd with { Floor = 200 };
Assert(!aptValidator.Validate(invalidFloorCmd).IsValid, "Floor > 150 should fail validation");

// ---------------------------------------------------------
// 5. Building Domain Model Fee Rates Tests
// ---------------------------------------------------------
Console.WriteLine("\n[5] Testing Building Domain configurable fee rates...");
var building = new Building
{
    Name = "Сграда А",
    Address = "бул. България 50",
    City = "София",
    FloorCount = 5,
    ApartmentCountPerFloor = 2,
    ManagerId = Guid.NewGuid()
};

Assert(building.BaseOccupantFee == 15.00m, "Default BaseOccupantFee should be 15.00 BGN");
Assert(building.PetFee == 15.00m, "Default PetFee should be 15.00 BGN");
Assert(building.RepairFundRatePerIdealPart == 1.50m, "Default RepairFundRatePerIdealPart should be 1.50 BGN");

building.BaseOccupantFee = 20.00m;
building.PetFee = 10.00m;
Assert(building.BaseOccupantFee == 20.00m, "Custom BaseOccupantFee should be modifiable");
Assert(building.PetFee == 10.00m, "Custom PetFee should be modifiable");

// ---------------------------------------------------------
// Summary
// ---------------------------------------------------------
Console.WriteLine("\n----------------------------------------");
Console.WriteLine($"Total Tests: {passed + failed} | Passed: {passed} | Failed: {failed}");
if (failed == 0)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("ALL TESTS PASSED SUCCESSFULLY! \u2714");
    Console.ResetColor();
    return 0;
}
else
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"{failed} TEST(S) FAILED! \u2716");
    Console.ResetColor();
    return 1;
}
