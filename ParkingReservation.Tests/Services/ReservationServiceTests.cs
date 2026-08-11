using Xunit;
using Moq;
using FluentAssertions;
using ParkingReservation.Application.DTOs;
using ParkingReservation.Application.Interfaces;
using ParkingReservation.Application.Services;
using ParkingReservation.Domain.Entities;

namespace ParkingReservation.Tests.Services;

public class ReservationServiceTests
{
    private readonly Mock<IParkingReservationRepository> _repositoryMock;
    private readonly ReservationService _service;

    public ReservationServiceTests()
    {
        _repositoryMock = new Mock<IParkingReservationRepository>();
        _service = new ReservationService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateReservationAsync_ValidRequest_ReturnsSuccess()
    {
        //Arrange
        var spot = new ParkingSpot { Id = 1, Code = "A1", IsActive = true };
        var request = new CreateReservationRequest
        {
            ParkingSpotId = 1,
            RequesterName = "Nagy Attila",
            StartTime = new DateTime(2026, 8, 20, 10, 0, 0),
            EndTime = new DateTime(2026, 8, 20, 12, 0, 0)
        };

        _repositoryMock.Setup(r => r.GetParkingSpotByIdAsync(1)).ReturnsAsync(spot);
        _repositoryMock.Setup(r => r.HasOverlappingReservationsAsync(1, request.StartTime, request.EndTime))
            .ReturnsAsync(false);

        //Act
        var (success, error, result) = await _service.CreateReservationAsync(request);

        //Assert
        success.Should().BeTrue();
        error.Should().BeNull();
        result.Should().NotBeNull();
        result!.ParkingSpotCode.Should().Be("A1");
        _repositoryMock.Verify(r => r.AddReservationAsync(It.IsAny<Reservation>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateReservationAsync_EndTimeBeforeStartTime_ReturnsFailure()
    {
        var request = new CreateReservationRequest
        {
            ParkingSpotId = 1,
            RequesterName = "Nagy Attila",
            StartTime = new DateTime(2026, 8, 20, 12, 0, 0),
            EndTime = new DateTime(2026, 8, 20, 10, 0, 0)
        };

        var (success, error, result) = await _service.CreateReservationAsync(request);

        success.Should().BeFalse();
        error.Should().Contain("záró időpontnak");
        result.Should().BeNull();
        _repositoryMock.Verify(r => r.AddReservationAsync(It.IsAny<Reservation>()), Times.Never);
    }

    [Fact]
    public async Task CreateReservationAsync_ParkingSpotDoesNotExist_ReturnsFailure()
    {
        var request = new CreateReservationRequest
        {
            ParkingSpotId = 99,
            RequesterName = "Nagy Attila",
            StartTime = new DateTime(2026, 8, 20, 10, 0, 0),
            EndTime = new DateTime(2026, 8, 20, 12, 0, 0)
        };

        _repositoryMock.Setup(r => r.GetParkingSpotByIdAsync(99)).ReturnsAsync((ParkingSpot?)null);

        var (success, error, result) = await _service.CreateReservationAsync(request);

        success.Should().BeFalse();
        error.Should().Contain("nem létezik");
    }

    [Fact]
    public async Task CreateReservationAsync_ParkingSpotInactive_ReturnsFailure()
    {
        var spot = new ParkingSpot { Id = 1, Code = "A1", IsActive = false };
        var request = new CreateReservationRequest
        {
            ParkingSpotId = 1,
            RequesterName = "Nagy Attila",
            StartTime = new DateTime(2026, 8, 20, 10, 0, 0),
            EndTime = new DateTime(2026, 8, 20, 12, 0, 0)
        };

        _repositoryMock.Setup(r => r.GetParkingSpotByIdAsync(1)).ReturnsAsync(spot);

        var (success, error, result) = await _service.CreateReservationAsync(request);

        success.Should().BeFalse();
        error.Should().Contain("nem foglalható");
    }

    [Fact]
    public async Task CreateReservationAsync_OverlappingReservationExists_ReturnsFailure()
    {
        var spot = new ParkingSpot { Id = 1, Code = "A1", IsActive = true };
        var request = new CreateReservationRequest
        {
            ParkingSpotId = 1,
            RequesterName = "Nagy Attila",
            StartTime = new DateTime(2026, 8, 20, 10, 0, 0),
            EndTime = new DateTime(2026, 8, 20, 12, 0, 0)
        };

        _repositoryMock.Setup(r => r.GetParkingSpotByIdAsync(1)).ReturnsAsync(spot);
        _repositoryMock.Setup(r => r.HasOverlappingReservationsAsync(1, request.StartTime, request.EndTime))
            .ReturnsAsync(true);

        var (success, error, result) = await _service.CreateReservationAsync(request);

        success.Should().BeFalse();
        error.Should().Contain("már foglalt");
        _repositoryMock.Verify(r => r.AddReservationAsync(It.IsAny<Reservation>()), Times.Never);
    }

    [Fact]
    public async Task CancelReservationAsync_ExistingConfirmedReservation_ReturnsSuccess()
    {
        var reservation = new Reservation
        {
            Id = 1,
            ParkingSpotId = 1,
            Status = ReservationStatus.Confirmed
        };

        _repositoryMock.Setup(r => r.GetReservationByIdAsync(1)).ReturnsAsync(reservation);

        var (success, error) = await _service.CancelReservationAsync(1);

        success.Should().BeTrue();
        reservation.Status.Should().Be(ReservationStatus.Cancelled);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CancelReservationAsync_ReservationNotFound_ReturnsFailure()
    {
        _repositoryMock.Setup(r => r.GetReservationByIdAsync(99)).ReturnsAsync((Reservation?)null);

        var (success, error) = await _service.CancelReservationAsync(99);

        success.Should().BeFalse();
        error.Should().Contain("nem található");
    }

    [Fact]
    public async Task CancelReservationAsync_AlreadyCancelled_ReturnsFailure()
    {
        var reservation = new Reservation
        {
            Id = 1,
            ParkingSpotId = 1,
            Status = ReservationStatus.Cancelled
        };

        _repositoryMock.Setup(r => r.GetReservationByIdAsync(1)).ReturnsAsync(reservation);

        var (success, error) = await _service.CancelReservationAsync(1);

        success.Should().BeFalse();
        error.Should().Contain("már le van mondva");
    }
}