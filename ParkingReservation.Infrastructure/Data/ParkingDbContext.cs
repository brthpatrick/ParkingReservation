using Microsoft.EntityFrameworkCore;
using ParkingReservation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingReservation.Infrastructure.Data;

public class ParkingDbContext : DbContext
{
    public ParkingDbContext(DbContextOptions<ParkingDbContext> options) : base(options)
    {
    }

    public DbSet<ParkingSpot> ParkingSpots => Set<ParkingSpot>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ParkingSpot>()
            .HasIndex(p => p.Code)
            .IsUnique();

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.ParkingSpot)
            .WithMany(p => p.Reservations)
            .HasForeignKey(r => r.ParkingSpotId);
    }
}
