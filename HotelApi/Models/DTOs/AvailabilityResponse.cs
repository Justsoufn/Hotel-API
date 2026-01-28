namespace HotelApi.Models.DTOs;

public class AvailabilityResponse
{
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int TotalAvailableRooms { get; set; }
    public List<RoomResponse> AvailableRooms { get; set; } = new();
}
