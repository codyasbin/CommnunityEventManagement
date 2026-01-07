using System.ComponentModel.DataAnnotations;

namespace CommnunityEventManagement.Data.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1")]
        public int Capacity { get; set; }

        public EventStatus Status { get; set; } = EventStatus.Upcoming;

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Foreign key for Venue
        [Required]
        public int VenueId { get; set; }
        public virtual Venue Venue { get; set; } = null!;

        // Many-to-many relationship with Activities
        public virtual ICollection<EventActivity> EventActivities { get; set; } = new List<EventActivity>();

        // One-to-many relationship with Registrations
        public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

        // Computed properties
        public int RegisteredCount => Registrations.Count(r => r.Status == RegistrationStatus.Confirmed);
        public int AvailableSpots => Capacity - RegisteredCount;
        public bool IsFull => AvailableSpots <= 0;
        public bool IsUpcoming => EventDate.Date >= DateTime.UtcNow.Date && Status == EventStatus.Upcoming;
    }

    public enum EventStatus
    {
        Upcoming,
        Ongoing,
        Completed,
        Cancelled
    }
}
