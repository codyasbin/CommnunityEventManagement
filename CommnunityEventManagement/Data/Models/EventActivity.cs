namespace CommnunityEventManagement.Data.Models
{
    // Join table for many-to-many relationship between Event and Activity
    public class EventActivity
    {
        public int EventId { get; set; }
        public virtual Event Event { get; set; } = null!;

        public int ActivityId { get; set; }
        public virtual Activity Activity { get; set; } = null!;
    }
}
