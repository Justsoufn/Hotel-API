namespace HotelApi.Models.Entities;

public class Reservation
{
    public Guid Id { get; set; }
    public int RoomId { get; set; }
    public string ConfirmationNumber { get; set; } = string.Empty;

    public string GuestFirstName { get; set; } = string.Empty;
    public string GuestLastName { get; set; } = string.Empty;
    public string GuestEmail { get; set; } = string.Empty;
    public string GuestPhone { get; set; } = string.Empty;

    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }

    public ReservationStatus Status { get; set; } = ReservationStatus.Confirmed;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CancelledAt { get; set; }

    public string? SpecialRequests { get; set; }

    public Room Room { get; set; } = null!;
}

public enum ReservationStatus
{
    Confirmed = 1,
    CheckedIn = 2,
    CheckedOut = 3,
    Cancelled = 4
}
