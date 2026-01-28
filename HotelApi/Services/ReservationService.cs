using HotelApi.Data;
using HotelApi.Models.DTOs;
using HotelApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelApi.Services;

public class ReservationService : IReservationService
{
    private readonly HotelDbContext _context;

    public ReservationService(HotelDbContext context)
    {
        _context = context;
    }

    public async Task<ReservationResponse> CreateReservationAsync(CreateReservationRequest request)
    {
        // Validate dates
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (request.CheckInDate < today)
            throw new InvalidOperationException("Check-in date cannot be in the past.");

        if (request.CheckOutDate <= request.CheckInDate)
            throw new InvalidOperationException("Check-out date must be after check-in date.");

        // Find the room
        var room = await _context.Rooms
            .FirstOrDefaultAsync(r => r.RoomNumber == request.RoomNumber && r.IsActive);

        if (room == null)
            throw new KeyNotFoundException($"Room {request.RoomNumber} not found.");

        // Check for overlapping reservations
        var hasOverlap = await _context.Reservations
            .Where(r => r.RoomId == room.Id)
            .Where(r => r.Status != ReservationStatus.Cancelled)
            .Where(r => r.CheckInDate < request.CheckOutDate && request.CheckInDate < r.CheckOutDate)
            .AnyAsync();

        if (hasOverlap)
            throw new InvalidOperationException($"Room {request.RoomNumber} is not available for the selected dates.");

        // Generate confirmation number
        var confirmationNumber = $"RES-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            RoomId = room.Id,
            ConfirmationNumber = confirmationNumber,
            GuestFirstName = request.GuestFirstName,
            GuestLastName = request.GuestLastName,
            GuestEmail = request.GuestEmail,
            GuestPhone = request.GuestPhone,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            SpecialRequests = request.SpecialRequests,
            Status = ReservationStatus.Confirmed,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return MapToResponse(reservation, room);
    }

    public async Task CancelReservationAsync(Guid id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Room)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
            throw new KeyNotFoundException($"Reservation {id} not found.");

        if (reservation.Status == ReservationStatus.Cancelled)
            throw new InvalidOperationException("Reservation is already cancelled.");

        if (reservation.Status == ReservationStatus.CheckedOut)
            throw new InvalidOperationException("Cannot cancel a completed reservation.");

        reservation.Status = ReservationStatus.Cancelled;
        reservation.CancelledAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task<ReservationResponse?> GetReservationByIdAsync(Guid id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Room)
            .FirstOrDefaultAsync(r => r.Id == id);

        return reservation == null ? null : MapToResponse(reservation, reservation.Room);
    }

    public async Task<ReservationResponse?> GetReservationByConfirmationNumberAsync(string confirmationNumber)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Room)
            .FirstOrDefaultAsync(r => r.ConfirmationNumber == confirmationNumber);

        return reservation == null ? null : MapToResponse(reservation, reservation.Room);
    }

    public async Task<IEnumerable<ReservationResponse>> GetReservationsByRoomNumberAsync(
        int roomNumber, bool includeCancelled, bool includePast)
    {
        var room = await _context.Rooms
            .FirstOrDefaultAsync(r => r.RoomNumber == roomNumber);

        if (room == null)
            throw new KeyNotFoundException($"Room {roomNumber} not found.");

        var query = _context.Reservations
            .Include(r => r.Room)
            .Where(r => r.RoomId == room.Id);

        if (!includeCancelled)
            query = query.Where(r => r.Status != ReservationStatus.Cancelled);

        if (!includePast)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            query = query.Where(r => r.CheckOutDate >= today);
        }

        var reservations = await query
            .OrderBy(r => r.CheckInDate)
            .ToListAsync();

        return reservations.Select(r => MapToResponse(r, r.Room));
    }

    public async Task<ReservationResponse> CheckInAsync(Guid id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Room)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
            throw new KeyNotFoundException($"Reservation {id} not found.");

        if (reservation.Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException($"Cannot check in. Current status: {reservation.Status}");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (reservation.CheckInDate > today)
            throw new InvalidOperationException("Cannot check in before the reservation start date.");

        reservation.Status = ReservationStatus.CheckedIn;
        await _context.SaveChangesAsync();

        return MapToResponse(reservation, reservation.Room);
    }

    public async Task<ReservationResponse> CheckOutAsync(Guid id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Room)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
            throw new KeyNotFoundException($"Reservation {id} not found.");

        if (reservation.Status != ReservationStatus.CheckedIn)
            throw new InvalidOperationException($"Cannot check out. Current status: {reservation.Status}");

        reservation.Status = ReservationStatus.CheckedOut;
        await _context.SaveChangesAsync();

        return MapToResponse(reservation, reservation.Room);
    }

    private static ReservationResponse MapToResponse(Reservation reservation, Room room)
    {
        var nights = reservation.CheckOutDate.DayNumber - reservation.CheckInDate.DayNumber;

        return new ReservationResponse
        {
            Id = reservation.Id,
            ConfirmationNumber = reservation.ConfirmationNumber,
            RoomNumber = room.RoomNumber,
            Floor = room.Floor,
            RoomType = room.Type.ToString(),
            CheckInDate = reservation.CheckInDate,
            CheckOutDate = reservation.CheckOutDate,
            NumberOfNights = nights,
            GuestName = $"{reservation.GuestFirstName} {reservation.GuestLastName}",
            GuestEmail = reservation.GuestEmail,
            GuestPhone = reservation.GuestPhone,
            Status = reservation.Status.ToString(),
            TotalPrice = room.PricePerNight * nights,
            CreatedAt = reservation.CreatedAt,
            SpecialRequests = reservation.SpecialRequests
        };
    }
}
