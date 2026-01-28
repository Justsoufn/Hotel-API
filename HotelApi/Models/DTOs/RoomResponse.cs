namespace HotelApi.Models.DTOs;

public class RoomResponse
{
    public int Id { get; set; }
    public int RoomNumber { get; set; }
    public int Floor { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
}
