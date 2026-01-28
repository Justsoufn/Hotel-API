using HotelApi.Models.DTOs;
using HotelApi.Models.Entities;
using HotelApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly IReservationService _reservationService;

    public RoomsController(IRoomService roomService, IReservationService reservationService)
    {
        _roomService = roomService;
        _reservationService = reservationService;
    }

    /// <summary>
    /// Get all rooms with optional filtering by floor and type
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RoomResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRooms([FromQuery] int? floor, [FromQuery] RoomType? type)
    {
        var rooms = await _roomService.GetAllRoomsAsync(floor, type);
        return Ok(rooms);
    }

    /// <summary>
    /// Get a specific room by room number
    /// </summary>
    [HttpGet("{roomNumber:int}")]
    [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoom(int roomNumber)
    {
        var room = await _roomService.GetRoomByNumberAsync(roomNumber);
        if (room == null)
            return NotFound($"Room {roomNumber} not found.");

        return Ok(room);
    }

    /// <summary>
    /// Search for available rooms for a date range
    /// </summary>
    [HttpGet("available")]
    [ProducesResponseType(typeof(AvailabilityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAvailableRooms([FromQuery] SearchAvailableRoomsRequest request)
    {
        if (request.CheckOutDate <= request.CheckInDate)
            return BadRequest("Check-out date must be after check-in date.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (request.CheckInDate < today)
            return BadRequest("Check-in date cannot be in the past.");

        var availability = await _roomService.GetAvailableRoomsAsync(request);
        return Ok(availability);
    }

    /// <summary>
    /// Get all reservations for a specific room
    /// </summary>
    [HttpGet("{roomNumber:int}/reservations")]
    [ProducesResponseType(typeof(IEnumerable<ReservationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoomReservations(
        int roomNumber,
        [FromQuery] bool includeCancelled = false,
        [FromQuery] bool includePast = false)
    {
        try
        {
            var reservations = await _reservationService.GetReservationsByRoomNumberAsync(
                roomNumber, includeCancelled, includePast);
            return Ok(reservations);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
