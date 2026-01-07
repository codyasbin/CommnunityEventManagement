using CommnunityEventManagement.Data.Models;

namespace CommnunityEventManagement.Services
{
    public class EventFilterModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? VenueId { get; set; }
        public int? ActivityId { get; set; }
        public EventStatus? Status { get; set; }
        public string? SearchTerm { get; set; }
    }

    public interface IEventService
    {
        Task<List<Event>> GetAllAsync();
        Task<List<Event>> GetUpcomingEventsAsync();
        Task<List<Event>> GetFilteredEventsAsync(EventFilterModel filter);
        Task<Event?> GetByIdAsync(int id);
        Task<Event> CreateAsync(Event evt, List<int> activityIds);
        Task<Event> UpdateAsync(Event evt, List<int> activityIds);
        Task<bool> DeleteAsync(int id);
        Task<bool> CanRegisterAsync(int eventId, string userId);
    }
}
