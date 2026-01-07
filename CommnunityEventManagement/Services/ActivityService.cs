using CommnunityEventManagement.Data;
using CommnunityEventManagement.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CommnunityEventManagement.Services
{
    public class ActivityService : IActivityService
    {
        private readonly ApplicationDbContext _context;

        public ActivityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Activity>> GetAllAsync()
        {
            return await _context.Activities
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<Activity?> GetByIdAsync(int id)
        {
            return await _context.Activities
                .Include(a => a.EventActivities)
                    .ThenInclude(ea => ea.Event)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Activity> CreateAsync(Activity activity)
        {
            activity.CreatedAt = DateTime.UtcNow;
            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();
            return activity;
        }

        public async Task<Activity> UpdateAsync(Activity activity)
        {
            var existing = await _context.Activities.FindAsync(activity.Id);
            if (existing == null)
                throw new ArgumentException("Activity not found");

            existing.Name = activity.Name;
            existing.Description = activity.Description;
            existing.Category = activity.Category;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
                return false;

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            return await _context.Activities
                .Where(a => !string.IsNullOrEmpty(a.Category))
                .Select(a => a.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }
    }
}
