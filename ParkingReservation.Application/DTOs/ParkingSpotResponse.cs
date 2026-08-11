using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingReservation.Application.DTOs;

public class ParkingSpotResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
