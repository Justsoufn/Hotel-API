using HotelApi.Data;
using HotelApi.Models.DTOs;
using HotelApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelApi.Services;

public class RoomService : IRoomService
{
    private readonly HotelDbContext _context;

    public RoomService(HotelDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RoomResponse>> GetAllRoomsAsync(int? floor, RoomType? type)
    {
        var query = _context.Rooms.Where(r => r.IsActive);

        if (floor.HasValue)
            query = query.Where(r => r.Floor == floor.Value);

        if (type.HasValue)
            query = query.Where(r => r.Type == type.Value);

        var rooms = await query.OrderBy(r => r.RoomNumber).ToListAsync();

        return rooms.Select(MapToResponse);
    }

    public async Task<RoomResponse?> GetRoomByNumberAsync(int roomNumber)
    {
        var room = await _context.Rooms
            .FirstOrDefaultAsync(r => r.RoomNumber == roomNumber && r.IsActive);

        return room == null ? null : MapToResponse(room);
    }

    public async Task<AvailabilityResponse> GetAvailableRoomsAsync(SearchAvailableRoomsRequest request)
    {
        var query = _context.Rooms.Where(r => r.IsActive);

        if (request.Floor.HasValue)
            query = query.Where(r => r.Floor == request.Floor.Value);

        if (request.RoomType.HasValue)
            query = query.Where(r => r.Type == request.RoomType.Value);

        // Get rooms that don't have overlapping reservations
        var bookedRoomIds = await _context.Reservations
            .Where(r => r.Status != ReservationStatus.Cancelled)
            .Where(r => r.CheckInDate < request.CheckOutDate && request.CheckInDate < r.CheckOutDate)
            .Select(r => r.RoomId)
            .Distinct()
            .ToListAsync();

        var availableRooms = await query
            .Where(r => !bookedRoomIds.Contains(r.Id))
            .OrderBy(r => r.RoomNumber)
            .ToListAsync();

        return new AvailabilityResponse
        {
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            TotalAvailableRooms = availableRooms.Count,
            AvailableRooms = availableRooms.Select(MapToResponse).ToList()
        };
    }

    private static RoomResponse MapToResponse(Room room)
    {
        return new RoomResponse
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            Floor = room.Floor,
            Type = room.Type.ToString(),
            PricePerNight = room.PricePerNight
        };
    }
}
