using CommnunityEventManagement.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CommnunityEventManagement.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventActivity> EventActivities { get; set; }
        public DbSet<Registration> Registrations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Venue configuration
            builder.Entity<Venue>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.Property(v => v.Name).IsRequired().HasMaxLength(100);
                entity.Property(v => v.Address).IsRequired().HasMaxLength(500);
                entity.Property(v => v.Description).HasMaxLength(1000);
            });

            // Activity configuration
            builder.Entity<Activity>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
                entity.Property(a => a.Description).HasMaxLength(1000);
                entity.Property(a => a.Category).HasMaxLength(50);
            });

            // Event configuration
            builder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);

                entity.HasOne(e => e.Venue)
                    .WithMany(v => v.Events)
                    .HasForeignKey(e => e.VenueId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Ignore(e => e.RegisteredCount);
                entity.Ignore(e => e.AvailableSpots);
                entity.Ignore(e => e.IsFull);
                entity.Ignore(e => e.IsUpcoming);
            });

            // EventActivity (join table) configuration
            builder.Entity<EventActivity>(entity =>
            {
                entity.HasKey(ea => new { ea.EventId, ea.ActivityId });

                entity.HasOne(ea => ea.Event)
                    .WithMany(e => e.EventActivities)
                    .HasForeignKey(ea => ea.EventId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ea => ea.Activity)
                    .WithMany(a => a.EventActivities)
                    .HasForeignKey(ea => ea.ActivityId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Registration configuration
            builder.Entity<Registration>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Notes).HasMaxLength(500);

                entity.HasOne(r => r.User)
                    .WithMany(u => u.Registrations)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Event)
                    .WithMany(e => e.Registrations)
                    .HasForeignKey(r => r.EventId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Unique constraint to prevent duplicate registrations
                entity.HasIndex(r => new { r.UserId, r.EventId }).IsUnique();
            });

            // ApplicationUser configuration
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FirstName).HasMaxLength(100);
                entity.Property(u => u.LastName).HasMaxLength(100);
            });
        }
    }
}
