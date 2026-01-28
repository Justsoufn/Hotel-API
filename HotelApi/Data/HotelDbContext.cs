using HotelApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelApi.Data;

public class HotelDbContext : DbContext
{
    public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
    {
    }

    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => r.RoomNumber).IsUnique();
            entity.HasIndex(r => r.Floor);
            entity.HasIndex(r => r.Type);
            entity.Property(r => r.PricePerNight).HasPrecision(10, 2);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => r.ConfirmationNumber).IsUnique();
            entity.HasIndex(r => new { r.RoomId, r.CheckInDate, r.CheckOutDate });
            entity.HasIndex(r => r.RoomId);
            entity.HasIndex(r => r.Status);

            entity.Property(r => r.ConfirmationNumber).HasMaxLength(20);
            entity.Property(r => r.GuestFirstName).HasMaxLength(100);
            entity.Property(r => r.GuestLastName).HasMaxLength(100);
            entity.Property(r => r.GuestEmail).HasMaxLength(255);
            entity.Property(r => r.GuestPhone).HasMaxLength(20);
            entity.Property(r => r.SpecialRequests).HasMaxLength(1000);

            entity.HasOne(r => r.Room)
                .WithMany(room => room.Reservations)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
