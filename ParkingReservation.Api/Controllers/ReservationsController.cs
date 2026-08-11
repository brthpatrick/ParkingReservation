using Microsoft.AspNetCore.Mvc;
using ParkingReservation.Application.DTOs;
using ParkingReservation.Application.Services;

namespace ParkingReservation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly ReservationService _reservationService;

    public ReservationsController(ReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost]
    public async Task<ActionResult<ReservationResponse>> Create([FromBody] CreateReservationRequest request)
    {
        var (succes, error, result) = await _reservationService.CreateReservationAsync(request);

        if (!succes)
        {
            return BadRequest(new { error });
        }

        return CreatedAtAction(nameof(Create), new { id = result!.Id }, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(int id)
    {
        var (success, error) = await _reservationService.CancelReservationAsync(id);

        if (!success)
        {
            return BadRequest(new { error });
        }

        return NoContent();
    }
}
