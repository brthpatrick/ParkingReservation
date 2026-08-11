using Microsoft.EntityFrameworkCore;
using ParkingReservation.Domain.Entities;
using ParkingReservation.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingReservation.Infrastructure.Repositories;

public class ParkingReservationRepository
{
    private readonly ParkingDbContext _context;

    public ParkingReservationRepository(ParkingDbContext context)
    {
        _context = context;
    }

    public async Task<List<Reservation>> GetReservationsByParkingSpotIdAsync(int parkingSpotId)
    {
        return await _context.Reservations
            .Where(r => r.ParkingSpotId == parkingSpotId)
            .Include(r => r.ParkingSpot)
            .OrderBy(r => r.StartTime)
            .ToListAsync();
    }

    public async Task<Reservation?> GetReservationByIdAsync(int id)
    {
        return await _context.Reservations
            .Include(r => r.ParkingSpot)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<bool> HasOverlappingReservationsAsync(int parkingSpotId, DateTime start, DateTime end)
    {
        return await _context.Reservations
            .Where(r => r.ParkingSpotId == parkingSpotId)
            .Where(r => r.Status == ReservationStatus.Confirmed)
            .AnyAsync(r => r.StartTime < end && r.EndTime > start);
    }   

    public async Task AddReservationAsync(Reservation reservation)
    {
        await _context.Reservations.AddAsync(reservation);
    }

    public async Task SaveChangesAsync() 
    {
        await _context.SaveChangesAsync();
    }
}
