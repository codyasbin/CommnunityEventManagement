using CommnunityEventManagement.Data;
using CommnunityEventManagement.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CommnunityEventManagement.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly ApplicationDbContext _context;

        public RegistrationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Registration?> GetByIdAsync(int id)
        {
            return await _context.Registrations
                .Include(r => r.Event)
                    .ThenInclude(e => e.Venue)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Registration>> GetUserRegistrationsAsync(string userId)
        {
            return await _context.Registrations
                .Include(r => r.Event)
                    .ThenInclude(e => e.Venue)
                .Include(r => r.Event)
                    .ThenInclude(e => e.EventActivities)
                        .ThenInclude(ea => ea.Activity)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Event.EventDate)
                .ToListAsync();
        }

        public async Task<List<Registration>> GetEventRegistrationsAsync(int eventId)
        {
            return await _context.Registrations
                .Include(r => r.User)
                .Where(r => r.EventId == eventId)
                .OrderBy(r => r.RegistrationDate)
                .ToListAsync();
        }

        public async Task<Registration> RegisterAsync(string userId, int eventId, string? notes = null)
        {
            // Check if event exists and has capacity
            var evt = await _context.Events
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (evt == null)
                throw new ArgumentException("Event not found");

            if (!evt.IsUpcoming)
                throw new InvalidOperationException("Cannot register for past or cancelled events");

            if (evt.IsFull)
                throw new InvalidOperationException("Event is at full capacity");

            // Check for existing registration
            var existing = await _context.Registrations
                .FirstOrDefaultAsync(r => r.UserId == userId && r.EventId == eventId && r.Status == RegistrationStatus.Confirmed);

            if (existing != null)
                throw new InvalidOperationException("You are already registered for this event");

            var registration = new Registration
            {
                UserId = userId,
                EventId = eventId,
                Notes = notes,
                RegistrationDate = DateTime.UtcNow,
                Status = RegistrationStatus.Confirmed
            };

            _context.Registrations.Add(registration);
            await _context.SaveChangesAsync();

            return registration;
        }

        public async Task<bool> CancelRegistrationAsync(int registrationId, string userId)
        {
            var registration = await _context.Registrations
                .FirstOrDefaultAsync(r => r.Id == registrationId && r.UserId == userId);

            if (registration == null)
                return false;

            registration.Status = RegistrationStatus.Cancelled;
            registration.CancelledAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsUserRegisteredAsync(string userId, int eventId)
        {
            return await _context.Registrations
                .AnyAsync(r => r.UserId == userId && r.EventId == eventId && r.Status == RegistrationStatus.Confirmed);
        }
    }
}
