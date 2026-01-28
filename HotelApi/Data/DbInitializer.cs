using HotelApi.Models.Entities;

namespace HotelApi.Data;

public static class DbInitializer
{
    public static void Initialize(HotelDbContext context)
    {
        if (context.Rooms.Any())
            return;

        var rooms = new List<Room>();

        for (int floor = 1; floor <= 9; floor++)
        {
            for (int roomIndex = 1; roomIndex <= 20; roomIndex++)
            {
                int roomNumber = floor * 100 + roomIndex;

                // Rooms 1-15: Standard, Rooms 16-20: Suite
                RoomType type = roomIndex <= 15 ? RoomType.Standard : RoomType.Suite;
                decimal price = type == RoomType.Standard ? 99.00m : 199.00m;

                rooms.Add(new Room
                {
                    RoomNumber = roomNumber,
                    Floor = floor,
                    Type = type,
                    PricePerNight = price,
                    IsActive = true
                });
            }
        }

        context.Rooms.AddRange(rooms);
        context.SaveChanges();
    }
}
