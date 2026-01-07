using CommnunityEventManagement.Data.Models;

namespace CommnunityEventManagement.Services
{
    public interface IRegistrationService
    {
        Task<Registration?> GetByIdAsync(int id);
        Task<List<Registration>> GetUserRegistrationsAsync(string userId);
        Task<List<Registration>> GetEventRegistrationsAsync(int eventId);
        Task<Registration> RegisterAsync(string userId, int eventId, string? notes = null);
        Task<bool> CancelRegistrationAsync(int registrationId, string userId);
        Task<bool> IsUserRegisteredAsync(string userId, int eventId);
    }
}
