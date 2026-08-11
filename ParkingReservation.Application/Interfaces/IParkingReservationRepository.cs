using ParkingReservation.Domain.Entities;

namespace ParkingReservation.Application.Interfaces;

public interface IParkingReservationRepository
{
   Task<List<ParkingSpot>> GetAllParkingSpotsAsync();
   Task<ParkingSpot?> GetParkingSpotByIdAsync(int id);

   Task<List<Reservation>> GetReservationsByParkingSpotIdAsync(int parkingSpotId);
   Task<Reservation?> GetReservationByIdAsync(int id);
   Task<bool> HasOverlappingReservationsAsync(int parkingSpotId, DateTime start, DateTime end);

   Task AddReservationAsync(Reservation reservation);
   Task SaveChangesAsync();
}