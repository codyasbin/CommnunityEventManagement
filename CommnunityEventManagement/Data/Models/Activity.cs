using System.ComponentModel.DataAnnotations;

namespace CommnunityEventManagement.Data.Models
{
    public class Activity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Many-to-many relationship with Events
        public virtual ICollection<EventActivity> EventActivities { get; set; } = new List<EventActivity>();
    }
}
