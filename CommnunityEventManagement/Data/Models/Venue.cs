using System.ComponentModel.DataAnnotations;

namespace CommnunityEventManagement.Data.Models
{
    public class Venue
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1")]
        public int MaxCapacity { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
