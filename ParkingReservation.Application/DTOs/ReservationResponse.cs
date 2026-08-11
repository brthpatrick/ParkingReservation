using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ParkingReservation.Application.DTOs;

public class ReservationResponse
{
    public int Id { get; set; }
    public int ParkingSpotId { get; set; }
    public string ParkingSpotCode { get; set; } = string.Empty;
    public string RequesterName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
}
