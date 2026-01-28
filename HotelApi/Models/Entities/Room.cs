namespace HotelApi.Models.Entities;

public class Room
{
    public int Id { get; set; }
    public int RoomNumber { get; set; }
    public int Floor { get; set; }
    public RoomType Type { get; set; }
    public decimal PricePerNight { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}

public enum RoomType
{
    Standard = 1,
    Suite = 2
}
