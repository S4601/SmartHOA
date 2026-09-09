using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHOA.Application.Fees.Commands.PayFee;
using SmartHOA.Application.Fees.Queries.GetUnpaidFees;

namespace SmartHOA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FeesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FeesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("unpaid/{apartmentId}")]
    public async Task<IActionResult> GetUnpaidFees(Guid apartmentId)
    {
        var fees = await _mediator.Send(new GetUnpaidFeesQuery(apartmentId));
        return Ok(fees);
    }
    
    [HttpPost("{feeId}/pay")]
    public async Task<IActionResult> PayFee(Guid feeId)
    {
        await _mediator.Send(new PayFeeCommand(feeId));
        return Ok(new { Message = "Fee Payed Successfully" });
    }
}

