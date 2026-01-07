using CommnunityEventManagement.Data;
using CommnunityEventManagement.Data.Models;
using CommnunityEventManagement.Services;
using CommnunityEventManagement.Tests.Helpers;

namespace CommnunityEventManagement.Tests.Services
{
    public class ActivityServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ActivityService _activityService;

        public ActivityServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _activityService = new ActivityService(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ReturnsAllActivities()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _activityService.GetAllAsync();

            // Assert
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoActivities()
        {
            // Act
            var result = await _activityService.GetAllAsync();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_OrdersByName()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _activityService.GetAllAsync();

            // Assert
            result.Should().BeInAscendingOrder(a => a.Name);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ReturnsActivity_WhenExists()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _activityService.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Workshop");
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _activityService.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_CreatesNewActivity()
        {
            // Arrange
            var newActivity = new Activity
            {
                Name = "New Activity",
                Category = "Test",
                Description = "A new activity"
            };

            // Act
            var result = await _activityService.CreateAsync(newActivity);

            // Assert
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("New Activity");
            result.Category.Should().Be("Test");
        }

        [Fact]
        public async Task CreateAsync_SetsCreatedAtToUtcNow()
        {
            // Arrange
            var beforeCreate = DateTime.UtcNow;
            var newActivity = new Activity
            {
                Name = "Test Activity",
                Category = "Test"
            };

            // Act
            var result = await _activityService.CreateAsync(newActivity);

            // Assert
            result.CreatedAt.Should().BeOnOrAfter(beforeCreate);
            result.CreatedAt.Should().BeOnOrBefore(DateTime.UtcNow);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_UpdatesActivityProperties()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);
            var activityToUpdate = new Activity
            {
                Id = 1,
                Name = "Updated Workshop",
                Category = "Updated Category",
                Description = "Updated description"
            };

            // Act
            var result = await _activityService.UpdateAsync(activityToUpdate);

            // Assert
            result.Name.Should().Be("Updated Workshop");
            result.Category.Should().Be("Updated Category");
            result.Description.Should().Be("Updated description");
            result.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_ThrowsException_WhenActivityNotFound()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);
            var nonExistentActivity = new Activity
            {
                Id = 999,
                Name = "Non-existent"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _activityService.UpdateAsync(nonExistentActivity));
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenActivityDeleted()
        {
            // Arrange
            var activity = new Activity
            {
                Name = "To Delete",
                Category = "Test"
            };
            await _activityService.CreateAsync(activity);

            // Act
            var result = await _activityService.DeleteAsync(activity.Id);

            // Assert
            result.Should().BeTrue();
            var deletedActivity = await _context.Activities.FindAsync(activity.Id);
            deletedActivity.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenActivityNotFound()
        {
            // Act
            var result = await _activityService.DeleteAsync(999);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region GetByCategoryAsync Tests

        [Fact]
        public async Task GetByCategoryAsync_ReturnsActivitiesInCategory()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _activityService.GetByCategoryAsync("Education");

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(a => a.Category == "Education");
        }

        [Fact]
        public async Task GetByCategoryAsync_ReturnsEmptyList_WhenCategoryNotFound()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _activityService.GetByCategoryAsync("NonExistent");

            // Assert
            result.Should().BeEmpty();
        }

        #endregion
    }
}
