using CommnunityEventManagement.Data;
using CommnunityEventManagement.Data.Models;
using CommnunityEventManagement.Services;
using CommnunityEventManagement.Tests.Helpers;

namespace CommnunityEventManagement.Tests.Services
{
    public class RegistrationServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly RegistrationService _registrationService;

        public RegistrationServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _registrationService = new RegistrationService(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ReturnsAllRegistrations()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _registrationService.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoRegistrations()
        {
            // Act
            var result = await _registrationService.GetAllAsync();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_OrdersByRegistrationDateDescending()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _registrationService.GetAllAsync();

            // Assert
            result.Should().BeInDescendingOrder(r => r.RegistrationDate);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ReturnsRegistration_WhenExists()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _registrationService.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.UserId.Should().Be("user-1");
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _registrationService.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_IncludesEventAndUser()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _registrationService.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Event.Should().NotBeNull();
            result.User.Should().NotBeNull();
        }

        #endregion

        #region GetUserRegistrationsAsync Tests

        [Fact]
        public async Task GetUserRegistrationsAsync_ReturnsUserRegistrations()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _registrationService.GetUserRegistrationsAsync("user-1");

            // Assert
            result.Should().HaveCount(1);
            result.Should().OnlyContain(r => r.UserId == "user-1");
        }

        [Fact]
        public async Task GetUserRegistrationsAsync_ReturnsEmptyList_WhenUserHasNoRegistrations()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _registrationService.GetUserRegistrationsAsync("admin-1");

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetUserRegistrationsAsync_OrdersByEventDateDescending()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Add another registration for user-1
            _context.Registrations.Add(new Registration
            {
                UserId = "user-1",
                EventId = 1,
                Status = RegistrationStatus.Confirmed
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _registrationService.GetUserRegistrationsAsync("user-1");

            // Assert
            result.Should().BeInDescendingOrder(r => r.Event.EventDate);
        }

        #endregion

        #region GetEventRegistrationsAsync Tests

        [Fact]
        public async Task GetEventRegistrationsAsync_ReturnsEventRegistrations()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _registrationService.GetEventRegistrationsAsync(4);

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(r => r.EventId == 4);
        }

        [Fact]
        public async Task GetEventRegistrationsAsync_ReturnsEmptyList_WhenEventHasNoRegistrations()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _registrationService.GetEventRegistrationsAsync(1);

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region RegisterAsync Tests

        [Fact]
        public async Task RegisterAsync_CreatesNewRegistration()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _registrationService.RegisterAsync("admin-1", 1, "Test notes");

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be("admin-1");
            result.EventId.Should().Be(1);
            result.Notes.Should().Be("Test notes");
            result.Status.Should().Be(RegistrationStatus.Confirmed);
        }

        [Fact]
        public async Task RegisterAsync_ThrowsException_WhenEventNotFound()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _registrationService.RegisterAsync("user-1", 999));
        }

        [Fact]
        public async Task RegisterAsync_ThrowsException_WhenEventIsFull()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act & Assert - Event 4 is full
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _registrationService.RegisterAsync("admin-1", 4));
        }

        [Fact]
        public async Task RegisterAsync_ThrowsException_WhenUserAlreadyRegistered()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);
            
            // First registration
            await _registrationService.RegisterAsync("admin-1", 1);

            // Act & Assert - Try to register again
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _registrationService.RegisterAsync("admin-1", 1));
        }

        [Fact]
        public async Task RegisterAsync_ThrowsException_WhenEventIsNotUpcoming()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act & Assert - Event 3 is past/completed
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _registrationService.RegisterAsync("admin-1", 3));
        }

        [Fact]
        public async Task RegisterAsync_SetsRegistrationDateToUtcNow()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);
            var beforeRegister = DateTime.UtcNow;

            // Act
            var result = await _registrationService.RegisterAsync("admin-1", 1);

            // Assert
            result.RegistrationDate.Should().BeOnOrAfter(beforeRegister);
            result.RegistrationDate.Should().BeOnOrBefore(DateTime.UtcNow);
        }

        #endregion

        #region CancelRegistrationAsync Tests

        [Fact]
        public async Task CancelRegistrationAsync_CancelsRegistration()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _registrationService.CancelRegistrationAsync(1, "user-1");

            // Assert
            result.Should().BeTrue();
            var registration = await _context.Registrations.FindAsync(1);
            registration!.Status.Should().Be(RegistrationStatus.Cancelled);
            registration.CancelledAt.Should().NotBeNull();
        }

        [Fact]
        public async Task CancelRegistrationAsync_ReturnsFalse_WhenRegistrationNotFound()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _registrationService.CancelRegistrationAsync(999, "user-1");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task CancelRegistrationAsync_ReturnsFalse_WhenUserDoesNotOwnRegistration()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act - user-2 trying to cancel user-1's registration
            var result = await _registrationService.CancelRegistrationAsync(1, "user-2");

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region IsUserRegisteredAsync Tests

        [Fact]
        public async Task IsUserRegisteredAsync_ReturnsTrue_WhenUserIsRegistered()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _registrationService.IsUserRegisteredAsync("user-1", 4);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsUserRegisteredAsync_ReturnsFalse_WhenUserIsNotRegistered()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);

            // Act
            var result = await _registrationService.IsUserRegisteredAsync("admin-1", 1);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsUserRegisteredAsync_ReturnsFalse_WhenRegistrationIsCancelled()
        {
            // Arrange
            await TestDbContextFactory.SeedTestDataAsync(_context);
            await _registrationService.CancelRegistrationAsync(1, "user-1");

            // Act
            var result = await _registrationService.IsUserRegisteredAsync("user-1", 4);

            // Assert
            result.Should().BeFalse();
        }

        #endregion
    }
}
