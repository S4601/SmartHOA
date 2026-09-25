using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHOA.Application.Apartments.Commands;
using SmartHOA.Application.Apartments.Queries.GetApartaments;

namespace SmartHOA.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ApartmentController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public ApartmentController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateApartmentCommand command)
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var currentUserRole = User.FindFirstValue(ClaimTypes.Role);
        
        var commandWithUserContext = command with 
        { 
            CurrentUserId = currentUserId, 
            CurrentUserRole = currentUserRole 
        };
        
        var apartmentId = await _mediator.Send(commandWithUserContext);
        
        return Ok(apartmentId);
    }
    
    
    [HttpGet("{id}")]
    [Authorize] 
    public async Task<IActionResult> GetApartment(Guid id)
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

        // Подаваме ги на Query-то
        var query = new GetApartmentQuery(id, currentUserId, currentUserRole);
        var apartment = await _mediator.Send(query);
    
        return Ok(apartment);
    }
    
    [HttpGet]
    [Authorize] 
    public async Task<IActionResult> GetApartments()
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

        // Подаваме ги на Query-то
        var query = new GetApartamentsQuery(currentUserId, currentUserRole);
        var apartment = await _mediator.Send(query);
    
        return Ok(apartment);
    }
    
}