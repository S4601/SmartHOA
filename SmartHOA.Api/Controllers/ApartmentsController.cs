using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHOA.Application.Apartments.Commands;
using SmartHOA.Application.Apartments.Queries.GetApartaments;

namespace SmartHOA.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[Route("api/Apartment")]
public class ApartmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public ApartmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateApartmentCommand command)
    {
        var apartmentId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetApartment), new { id = apartmentId }, apartmentId);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetApartment(Guid id)
    {
        var query = new GetApartmentQuery(id);
        var apartment = await _mediator.Send(query);
    
        return Ok(apartment);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetApartments()
    {
        var query = new GetApartamentsQuery();
        var apartments = await _mediator.Send(query);
    
        return Ok(apartments);
    }
}