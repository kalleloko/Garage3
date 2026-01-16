using Garage3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Garage3.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Vehicle> Vehicles { get; set; } = default!;
        public DbSet<ParkingSpot> ParkingSpots { get; set; } = default!;
        public DbSet<Parking> Parkings { get; set; } = default!;
        public DbSet<VehicleType> VehicleTypes { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Vehicle>()
            //    .Property(p => p.ArrivalTime)
            //    .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Owner)
                .WithMany(u => u.Vehicles)
                .HasForeignKey(v => v.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Parking>()
                .HasIndex(p => new { p.VehicleId, p.DepartTime })
                .HasFilter("[DepartTime] IS NULL"); // Ensures a vehicle can have only one active parking

            modelBuilder.Entity<Parking>()
                .HasOne(p => p.Vehicle)
                .WithMany(v => v.Parkings)
                .HasForeignKey(p => p.VehicleId);

            modelBuilder.Entity<Parking>()
                .HasOne(p => p.ParkingSpot)
                .WithMany(ps => ps.Parkings)
                .HasForeignKey(p => p.ParkingSpotId);

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.SSN)
                .IsUnique();

        }
    }


}
