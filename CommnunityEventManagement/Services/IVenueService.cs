using CommnunityEventManagement.Data.Models;

namespace CommnunityEventManagement.Services
{
    public interface IVenueService
    {
        Task<List<Venue>> GetAllAsync();
        Task<Venue?> GetByIdAsync(int id);
        Task<Venue> CreateAsync(Venue venue);
        Task<Venue> UpdateAsync(Venue venue);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasEventsAsync(int id);
    }
}
