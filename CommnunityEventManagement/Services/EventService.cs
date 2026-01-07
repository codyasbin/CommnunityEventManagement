using CommnunityEventManagement.Data;
using CommnunityEventManagement.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CommnunityEventManagement.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Event>> GetAllAsync()
        {
            return await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventActivities)
                    .ThenInclude(ea => ea.Activity)
                .Include(e => e.Registrations)
                .OrderByDescending(e => e.EventDate)
                .ToListAsync();
        }

        public async Task<List<Event>> GetUpcomingEventsAsync()
        {
            var today = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
            return await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventActivities)
                    .ThenInclude(ea => ea.Activity)
                .Include(e => e.Registrations)
                .Where(e => e.EventDate >= today && e.Status == EventStatus.Upcoming)
                .OrderBy(e => e.EventDate)
                .ThenBy(e => e.StartTime)
                .ToListAsync();
        }

        public async Task<List<Event>> GetFilteredEventsAsync(EventFilterModel filter)
        {
            var query = _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventActivities)
                    .ThenInclude(ea => ea.Activity)
                .Include(e => e.Registrations)
                .AsQueryable();

            if (filter.FromDateUtc.HasValue)
                query = query.Where(e => e.EventDate >= filter.FromDateUtc.Value);

            if (filter.ToDateUtc.HasValue)
                query = query.Where(e => e.EventDate <= filter.ToDateUtc.Value);

            if (filter.VenueId.HasValue)
                query = query.Where(e => e.VenueId == filter.VenueId.Value);

            if (filter.ActivityId.HasValue)
                query = query.Where(e => e.EventActivities.Any(ea => ea.ActivityId == filter.ActivityId.Value));

            if (filter.Status.HasValue)
                query = query.Where(e => e.Status == filter.Status.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                query = query.Where(e => e.Name.ToLower().Contains(filter.SearchTerm.ToLower()) ||
                                         e.Description.ToLower().Contains(filter.SearchTerm.ToLower()));

            return await query
                .OrderBy(e => e.EventDate)
                .ThenBy(e => e.StartTime)
                .ToListAsync();
        }

        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventActivities)
                    .ThenInclude(ea => ea.Activity)
                .Include(e => e.Registrations)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Event> CreateAsync(Event evt, List<int> activityIds)
        {
            // Ensure EventDate is UTC
            evt.EventDate = DateTime.SpecifyKind(evt.EventDate, DateTimeKind.Utc);
            evt.CreatedAt = DateTime.UtcNow;

            foreach (var activityId in activityIds)
            {
                evt.EventActivities.Add(new EventActivity { ActivityId = activityId });
            }

            _context.Events.Add(evt);
            await _context.SaveChangesAsync();
            return evt;
        }

        public async Task<Event> UpdateAsync(Event evt, List<int> activityIds)
        {
            var existing = await _context.Events
                .Include(e => e.EventActivities)
                .FirstOrDefaultAsync(e => e.Id == evt.Id);

            if (existing == null)
                throw new ArgumentException("Event not found");

            existing.Name = evt.Name;
            existing.Description = evt.Description;
            existing.EventDate = DateTime.SpecifyKind(evt.EventDate, DateTimeKind.Utc);
            existing.StartTime = evt.StartTime;
            existing.EndTime = evt.EndTime;
            existing.Capacity = evt.Capacity;
            existing.Status = evt.Status;
            existing.VenueId = evt.VenueId;
            existing.ImageUrl = evt.ImageUrl;
            existing.UpdatedAt = DateTime.UtcNow;

            // Update activities
            existing.EventActivities.Clear();
            foreach (var activityId in activityIds)
            {
                existing.EventActivities.Add(new EventActivity { EventId = existing.Id, ActivityId = activityId });
            }

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt == null)
                return false;

            _context.Events.Remove(evt);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CanRegisterAsync(int eventId, string userId)
        {
            var evt = await _context.Events
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (evt == null)
                return false;

            // Check if event is upcoming and has capacity
            if (!evt.IsUpcoming || evt.IsFull)
                return false;

            // Check if user is already registered
            var existingRegistration = evt.Registrations
                .FirstOrDefault(r => r.UserId == userId && r.Status == RegistrationStatus.Confirmed);

            return existingRegistration == null;
        }
    }
}
