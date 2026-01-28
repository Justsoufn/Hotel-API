using HotelApi.Models.DTOs;

namespace HotelApi.Services;

public interface IReservationService
{
    Task<ReservationResponse> CreateReservationAsync(CreateReservationRequest request);
    Task CancelReservationAsync(Guid id);
    Task<ReservationResponse?> GetReservationByIdAsync(Guid id);
    Task<ReservationResponse?> GetReservationByConfirmationNumberAsync(string confirmationNumber);
    Task<IEnumerable<ReservationResponse>> GetReservationsByRoomNumberAsync(int roomNumber, bool includeCancelled, bool includePast);
    Task<ReservationResponse> CheckInAsync(Guid id);
    Task<ReservationResponse> CheckOutAsync(Guid id);
}
