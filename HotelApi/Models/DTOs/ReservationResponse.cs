namespace HotelApi.Models.DTOs;

public class ReservationResponse
{
    public Guid Id { get; set; }
    public string ConfirmationNumber { get; set; } = string.Empty;
    public int RoomNumber { get; set; }
    public int Floor { get; set; }
    public string RoomType { get; set; } = string.Empty;
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int NumberOfNights { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public string GuestEmail { get; set; } = string.Empty;
    public string GuestPhone { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? SpecialRequests { get; set; }
}
