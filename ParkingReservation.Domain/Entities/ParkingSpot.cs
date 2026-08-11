namespace ParkingReservation.Domain.Entities;

public class ParkingSpot
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty; // pl. "A1"
    public bool IsActive { get; set; } = true;

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
