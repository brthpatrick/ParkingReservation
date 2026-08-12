using ParkingReservation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingReservation.Infrastructure.Data;

public static class DbSeeder
{
    public static void Seed(ParkingDbContext context)
    {
        if (context.ParkingSpots.Any())
        {
            return;
        }

        var spots = new List<ParkingSpot>
        {
            new() { Code = "A1", IsActive = true, Type = ParkingSpotType.Standard },
            new() { Code = "A2", IsActive = true, Type = ParkingSpotType.Standard },
            new() { Code = "A3", IsActive = true, Type = ParkingSpotType.Standard },
            new() { Code = "B1", IsActive = true, Type = ParkingSpotType.Disabled },
            new() { Code = "B2", IsActive = true, Type = ParkingSpotType.ElectricCharging },
        };

        context.ParkingSpots.AddRange(spots);
        context.SaveChanges();
    }
}