using HotelApi.Models.DTOs;
using HotelApi.Models.Entities;

namespace HotelApi.Services;

public interface IRoomService
{
    Task<IEnumerable<RoomResponse>> GetAllRoomsAsync(int? floor, RoomType? type);
    Task<RoomResponse?> GetRoomByNumberAsync(int roomNumber);
    Task<AvailabilityResponse> GetAvailableRoomsAsync(SearchAvailableRoomsRequest request);
}
