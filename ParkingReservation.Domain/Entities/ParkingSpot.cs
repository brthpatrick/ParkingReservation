namespace ParkingReservation.Domain.Entities;

public enum ParkingSpotType
{ 
    Standard = 0,
    Disabled = 1,
    ElectricCharging = 2,
}
public class ParkingSpot
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ParkingSpotType Type { get; set; } = ParkingSpotType.Standard;

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
