using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHOA.Application.Buildings.Commands.CreateBuilding;
using SmartHOA.Application.Buildings.Queries.GetBuildingById;
using SmartHOA.Application.Buildings.Queries.GetBuildings;

namespace SmartHOA.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BuildingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BuildingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateBuildingCommand command)
    {
        var buildingId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = buildingId }, buildingId);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetBuildingsQuery();
        var buildings = await _mediator.Send(query);
        return Ok(buildings);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetBuildingByIdQuery(id);
        var building = await _mediator.Send(query);
        return Ok(building);
    }
}