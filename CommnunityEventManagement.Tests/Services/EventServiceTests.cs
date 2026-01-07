using CommnunityEventManagement.Data;
using CommnunityEventManagement.Data.Models;
using CommnunityEventManagement.Services;
using CommnunityEventManagement.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CommnunityEventManagement.Tests.Services
{
    public class EventServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly EventService _eventService;

        public EventServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _eventService = new EventService(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ReturnsAllEvents()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _eventService.GetAllAsync();

            // Assert
            result.Should().HaveCount(4);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoEvents()
        {
            // Act
            var result = await _eventService.GetAllAsync();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_IncludesVenueAndActivities()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _eventService.GetAllAsync();

            // Assert
            var eventWithVenue = result.First(e => e.Id == 1);
            eventWithVenue.Venue.Should().NotBeNull();
            eventWithVenue.Venue.Name.Should().Be("Main Hall");
            eventWithVenue.EventActivities.Should().HaveCount(2);
        }

        #endregion

        #region GetUpcomingEventsAsync Tests

        [Fact]
        public async Task GetUpcomingEventsAsync_ReturnsOnlyUpcomingEvents()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _eventService.GetUpcomingEventsAsync();

            // Assert
            result.Should().OnlyContain(e => e.Status == EventStatus.Upcoming);
            result.Should().OnlyContain(e => e.EventDate >= DateTime.UtcNow.Date);
        }

        [Fact]
        public async Task GetUpcomingEventsAsync_ExcludesPastEvents()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _eventService.GetUpcomingEventsAsync();

            // Assert
            result.Should().NotContain(e => e.Name == "Past Event");
        }

        [Fact]
        public async Task GetUpcomingEventsAsync_OrdersByDateAndTime()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _eventService.GetUpcomingEventsAsync();

            // Assert
            result.Should().BeInAscendingOrder(e => e.EventDate);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ReturnsEvent_WhenExists()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _eventService.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Tech Conference 2025");
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _eventService.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_IncludesAllRelatedData()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _eventService.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Venue.Should().NotBeNull();
            result.EventActivities.Should().NotBeEmpty();
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_CreatesNewEvent()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);
            var newEvent = new Event
            {
                Name = "New Test Event",
                Description = "Test description",
                EventDate = DateTime.UtcNow.AddDays(60),
                StartTime = new TimeSpan(10, 0, 0),
                Capacity = 50,
                VenueId = 1
            };

            // Act
            var result = await _eventService.CreateAsync(newEvent, new List<int> { 1, 2 });

            // Assert
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("New Test Event");
            result.EventActivities.Should().HaveCount(2);
        }

        [Fact]
        public async Task CreateAsync_SetsCreatedAtToUtcNow()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);
            var beforeCreate = DateTime.UtcNow;
            var newEvent = new Event
            {
                Name = "New Event",
                Description = "Description",
                EventDate = DateTime.UtcNow.AddDays(30),
                StartTime = new TimeSpan(9, 0, 0),
                Capacity = 25,
                VenueId = 1
            };

            // Act
            var result = await _eventService.CreateAsync(newEvent, new List<int>());

            // Assert
            result.CreatedAt.Should().BeOnOrAfter(beforeCreate);
            result.CreatedAt.Should().BeOnOrBefore(DateTime.UtcNow);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_UpdatesEventProperties()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);
            var eventToUpdate = new Event
            {
                Id = 1,
                Name = "Updated Event Name",
                Description = "Updated description",
                EventDate = DateTime.UtcNow.AddDays(45),
                StartTime = new TimeSpan(11, 0, 0),
                Capacity = 200,
                VenueId = 2,
                Status = EventStatus.Upcoming
            };

            // Act
            var result = await _eventService.UpdateAsync(eventToUpdate, new List<int> { 2 });

            // Assert
            result.Name.Should().Be("Updated Event Name");
            result.Description.Should().Be("Updated description");
            result.Capacity.Should().Be(200);
            result.VenueId.Should().Be(2);
            result.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_ThrowsException_WhenEventNotFound()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);
            var nonExistentEvent = new Event
            {
                Id = 999,
                Name = "Non-existent",
                Description = "Does not exist",
                EventDate = DateTime.UtcNow,
                StartTime = new TimeSpan(9, 0, 0),
                Capacity = 10,
                VenueId = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _eventService.UpdateAsync(nonExistentEvent, new List<int>()));
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenEventDeleted()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _eventService.DeleteAsync(1);

            // Assert
            result.Should().BeTrue();
            var deletedEvent = await _context.Events.FindAsync(1);
            deletedEvent.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenEventNotFound()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _eventService.DeleteAsync(999);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region CanRegisterAsync Tests

        [Fact]
        public async Task CanRegisterAsync_ReturnsTrue_WhenUserCanRegister()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _eventService.CanRegisterAsync(1, "user-1");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task CanRegisterAsync_ReturnsFalse_WhenEventIsFull()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act - Event 4 is full (capacity 2, 2 registrations)
            var result = await _eventService.CanRegisterAsync(4, "admin-1");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task CanRegisterAsync_ReturnsFalse_WhenEventNotFound()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _eventService.CanRegisterAsync(999, "user-1");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task CanRegisterAsync_ReturnsFalse_WhenUserAlreadyRegistered()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act - user-1 is already registered for event 4
            var result = await _eventService.CanRegisterAsync(4, "user-1");

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region GetFilteredEventsAsync Tests

        [Fact]
        public async Task GetFilteredEventsAsync_FiltersByDateRange()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);
            var filter = new EventFilterModel
            {
                FromDate = DateTime.UtcNow.AddDays(5),
                ToDate = DateTime.UtcNow.AddDays(10)
            };

            // Act
            var result = await _eventService.GetFilteredEventsAsync(filter);

            // Assert
            result.Should().OnlyContain(e => e.EventDate >= filter.FromDateUtc && e.EventDate <= filter.ToDateUtc);
        }

        [Fact]
        public async Task GetFilteredEventsAsync_FiltersByVenue()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);
            var filter = new EventFilterModel { VenueId = 1 };

            // Act
            var result = await _eventService.GetFilteredEventsAsync(filter);

            // Assert
            result.Should().OnlyContain(e => e.VenueId == 1);
        }

        [Fact]
        public async Task GetFilteredEventsAsync_FiltersBySearchTerm()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);
            var filter = new EventFilterModel { SearchTerm = "Conference" };

            // Act
            var result = await _eventService.GetFilteredEventsAsync(filter);

            // Assert
            result.Should().Contain(e => e.Name.Contains("Conference"));
        }

        [Fact]
        public async Task GetFilteredEventsAsync_FiltersByStatus()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);
            var filter = new EventFilterModel { Status = EventStatus.Completed };

            // Act
            var result = await _eventService.GetFilteredEventsAsync(filter);

            // Assert
            result.Should().OnlyContain(e => e.Status == EventStatus.Completed);
        }

        #endregion
    }
}
