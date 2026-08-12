using ParkingReservation.Application.DTOs;
using ParkingReservation.Application.Interfaces;
using ParkingReservation.Domain.Entities;

namespace ParkingReservation.Application.Services;

public class ReservationService
{
    private readonly IParkingReservationRepository _repository;

    public ReservationService(IParkingReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<(bool Success, string? Error, ReservationResponse? Result)> CreateReservationAsync(CreateReservationRequest request)
    {
        if (request.EndTime <= request.StartTime)
        {
            return (false, "A záró időpontnak a kezdő időpont után kell lennie.", null);
        }

        var parkingSpot = await _repository.GetParkingSpotByIdAsync(request.ParkingSpotId);
        if (parkingSpot is null)
        {
            return (false, "A megadott parkolóhely nem létezik.", null);
        }
        if (!parkingSpot.IsActive)
        {
            return (false, "A megadott parkolóhely jelenleg nem foglalható.", null);
        }

        if (parkingSpot.Type == ParkingSpotType.Disabled && !request.HasDisabilityPermit)
        {
            return (false, "Ez a parkolóhely mozgáskorlátozottak számára van fenntartva, érvényes igazolvány szükséges a foglaláshoz.", null);
        }

        if (parkingSpot.Type == ParkingSpotType.ElectricCharging)
        {
            var duration = request.EndTime - request.StartTime;
            if (duration > TimeSpan.FromHours(4))
            {
                return (false, "Elektromos töltős parkolóhely egyszerre maximum 4 órára foglalható, a nagyobb kihasználtság érdekében.", null);
            }
        }

        var hasOverlap = await _repository.HasOverlappingReservationsAsync(
            request.ParkingSpotId, request.StartTime, request.EndTime);

        if (hasOverlap)
        {
            return (false, "A parkolóhely a megadott időszakban már foglalt.", null);
        }

        var reservation = new Reservation
        {
            ParkingSpotId = request.ParkingSpotId,
            RequesterName = request.RequesterName,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Status = ReservationStatus.Confirmed,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddReservationAsync(reservation);
        await _repository.SaveChangesAsync();

        var response = new ReservationResponse
        {
            Id = reservation.Id,
            ParkingSpotId = reservation.ParkingSpotId,
            ParkingSpotCode = parkingSpot.Code,
            RequesterName = reservation.RequesterName,
            StartTime = reservation.StartTime,
            EndTime = reservation.EndTime,
            Status = reservation.Status.ToString()
        };

        return (true, null, response);
    }

    public async Task<(bool Success, string? Error)> CancelReservationAsync(int reservationId)
    {
        var reservation = await _repository.GetReservationByIdAsync(reservationId);
        if (reservation is null)
        {
            return (false, "A foglalás nem található.");
        }

        if (reservation.Status == ReservationStatus.Cancelled)
        {
            return (false, "A foglalás már le van mondva.");
        }

        reservation.Status = ReservationStatus.Cancelled;
        await _repository.SaveChangesAsync();

        return (true, null);
    }
}