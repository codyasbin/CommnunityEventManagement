using System.ComponentModel.DataAnnotations;

namespace CommnunityEventManagement.Data.Models
{
    public class Registration
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

        [Required]
        public int EventId { get; set; }
        public virtual Event Event { get; set; } = null!;

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        public RegistrationStatus Status { get; set; } = RegistrationStatus.Confirmed;

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime? CancelledAt { get; set; }
    }

    public enum RegistrationStatus
    {
        Confirmed,
        Cancelled,
        Waitlisted
    }
}
