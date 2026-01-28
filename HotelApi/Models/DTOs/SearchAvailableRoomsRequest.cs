using HotelApi.Models.Entities;

namespace HotelApi.Models.DTOs;

public class SearchAvailableRoomsRequest
{
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public RoomType? RoomType { get; set; }
    public int? Floor { get; set; }
}
