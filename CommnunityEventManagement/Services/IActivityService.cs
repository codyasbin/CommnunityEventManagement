using CommnunityEventManagement.Data.Models;

namespace CommnunityEventManagement.Services
{
    public interface IActivityService
    {
        Task<List<Activity>> GetAllAsync();
        Task<Activity?> GetByIdAsync(int id);
        Task<Activity> CreateAsync(Activity activity);
        Task<Activity> UpdateAsync(Activity activity);
        Task<bool> DeleteAsync(int id);
        Task<List<string>> GetCategoriesAsync();
        Task<List<Activity>> GetByCategoryAsync(string category);
    }
}
