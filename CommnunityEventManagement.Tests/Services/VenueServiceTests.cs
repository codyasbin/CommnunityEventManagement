using CommnunityEventManagement.Data;
using CommnunityEventManagement.Data.Models;
using CommnunityEventManagement.Services;
using CommnunityEventManagement.Tests.Helpers;

namespace CommnunityEventManagement.Tests.Services
{
    public class VenueServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly VenueService _venueService;

        public VenueServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _venueService = new VenueService(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ReturnsAllVenues()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _venueService.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoVenues()
        {
            // Act
            var result = await _venueService.GetAllAsync();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_OrdersByName()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _venueService.GetAllAsync();

            // Assert
            result.Should().BeInAscendingOrder(v => v.Name);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ReturnsVenue_WhenExists()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _venueService.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Main Hall");
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _venueService.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_IncludesEvents()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _venueService.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Events.Should().NotBeNull();
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_CreatesNewVenue()
        {
            // Arrange
            var newVenue = new Venue
            {
                Name = "New Venue",
                Address = "789 New Street",
                MaxCapacity = 200,
                Description = "A new venue"
            };

            // Act
            var result = await _venueService.CreateAsync(newVenue);

            // Assert
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("New Venue");
            result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task CreateAsync_SetsCreatedAtToUtcNow()
        {
            // Arrange
            var beforeCreate = DateTime.UtcNow;
            var newVenue = new Venue
            {
                Name = "Test Venue",
                Address = "Test Address",
                MaxCapacity = 100
            };

            // Act
            var result = await _venueService.CreateAsync(newVenue);

            // Assert
            result.CreatedAt.Should().BeOnOrAfter(beforeCreate);
            result.CreatedAt.Should().BeOnOrBefore(DateTime.UtcNow);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_UpdatesVenueProperties()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);
            var venueToUpdate = new Venue
            {
                Id = 1,
                Name = "Updated Hall",
                Address = "Updated Address",
                MaxCapacity = 600,
                Description = "Updated description"
            };

            // Act
            var result = await _venueService.UpdateAsync(venueToUpdate);

            // Assert
            result.Name.Should().Be("Updated Hall");
            result.Address.Should().Be("Updated Address");
            result.MaxCapacity.Should().Be(600);
            result.Description.Should().Be("Updated description");
            result.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_ThrowsException_WhenVenueNotFound()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);
            var nonExistentVenue = new Venue
            {
                Id = 999,
                Name = "Non-existent",
                Address = "Nowhere",
                MaxCapacity = 100
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _venueService.UpdateAsync(nonExistentVenue));
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenVenueDeleted()
        {
            // Arrange
            var venue = new Venue
            {
                Name = "To Delete",
                Address = "Delete Address",
                MaxCapacity = 50
            };
            await _venueService.CreateAsync(venue);

            // Act
            var result = await _venueService.DeleteAsync(venue.Id);

            // Assert
            result.Should().BeTrue();
            var deletedVenue = await _context.Venues.FindAsync(venue.Id);
            deletedVenue.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenVenueNotFound()
        {
            // Act
            var result = await _venueService.DeleteAsync(999);

            // Assert
            result.Should().BeFalse();
        }

        #endregion
    }
}
