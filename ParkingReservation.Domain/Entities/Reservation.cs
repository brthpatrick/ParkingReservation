namespace ParkingReservation.Domain.Entities;

public enum ReservationStatus
{
    Confirmed = 0,
    Cancelled = 1
}

public class Reservation
{
    public int Id { get; set; }
    public int ParkingSpotId { get; set; }
    public ParkingSpot? ParkingSpot { get; set; }

    public string RequesterName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public ReservationStatus Status { get; set; } = ReservationStatus.Confirmed;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
