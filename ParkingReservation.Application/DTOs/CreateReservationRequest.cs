using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingReservation.Application.DTOs;

public class CreateReservationRequest
{
    public int ParkingSpotId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool HasDisabilityPermit { get; set; } = false;
}
