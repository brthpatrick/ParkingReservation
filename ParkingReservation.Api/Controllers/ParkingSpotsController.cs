using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkingReservation.Application.DTOs;
using ParkingReservation.Application.Interfaces;

namespace ParkingReservation.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ParkingSpotsController : ControllerBase
{
    private readonly IParkingReservationRepository _repository;

    public ParkingSpotsController(IParkingReservationRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<ParkingSpotResponse>>> GetAll()
    {
        var spots = await _repository.GetAllParkingSpotsAsync();

        var response = spots.Select(s => new ParkingSpotResponse
        {
            Id = s.Id,
            Code = s.Code,
            IsActive = s.IsActive,
            Type = s.Type.ToString()
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{id}/reservations")]
    public async Task<ActionResult<List<ReservationResponse>>> GetReservationsForSpot(int id)
    {
        var spot = await _repository.GetParkingSpotByIdAsync(id);
        if (spot is null)
        {
            return NotFound($"Parkolóhely nem található: {id}");
        }

        var reservations = await _repository.GetReservationsByParkingSpotIdAsync(id);

        var response = reservations.Select(r => new ReservationResponse
        {
            Id = r.Id,
            ParkingSpotId = r.ParkingSpotId,
            ParkingSpotCode = spot.Code,
            RequesterName = r.RequesterName,
            StartTime = r.StartTime,
            EndTime = r.EndTime,
            Status = r.Status.ToString(),
        }).ToList();

        return Ok(response);
    }
}
