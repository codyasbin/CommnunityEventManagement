using CommnunityEventManagement.Data;
using CommnunityEventManagement.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CommnunityEventManagement.Services
{
    public class VenueService : IVenueService
    {
        private readonly ApplicationDbContext _context;

        public VenueService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Venue>> GetAllAsync()
        {
            return await _context.Venues
                .OrderBy(v => v.Name)
                .ToListAsync();
        }

        public async Task<Venue?> GetByIdAsync(int id)
        {
            return await _context.Venues
                .Include(v => v.Events)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Venue> CreateAsync(Venue venue)
        {
            venue.CreatedAt = DateTime.UtcNow;
            _context.Venues.Add(venue);
            await _context.SaveChangesAsync();
            return venue;
        }

        public async Task<Venue> UpdateAsync(Venue venue)
        {
            var existing = await _context.Venues.FindAsync(venue.Id);
            if (existing == null)
                throw new ArgumentException("Venue not found");

            existing.Name = venue.Name;
            existing.Address = venue.Address;
            existing.MaxCapacity = venue.MaxCapacity;
            existing.Description = venue.Description;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null)
                return false;

            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasEventsAsync(int id)
        {
            return await _context.Events.AnyAsync(e => e.VenueId == id);
        }
    }
}
