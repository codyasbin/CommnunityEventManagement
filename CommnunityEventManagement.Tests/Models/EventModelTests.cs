using CommnunityEventManagement.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace CommnunityEventManagement.Tests.Models
{
    public class EventModelTests
    {
        #region Validation Tests

        [Fact]
        public void Event_WithValidData_PassesValidation()
        {
            // Arrange
            var evt = new Event
            {
                Name = "Valid Event",
                Description = "A valid description",
                EventDate = DateTime.UtcNow.AddDays(30),
                StartTime = new TimeSpan(9, 0, 0),
                Capacity = 50,
                VenueId = 1
            };

            // Act
            var validationResults = ValidateModel(evt);

            // Assert
            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void Event_WithEmptyName_FailsValidation()
        {
            // Arrange
            var evt = new Event
            {
                Name = "",
                Description = "Description",
                EventDate = DateTime.UtcNow.AddDays(30),
                StartTime = new TimeSpan(9, 0, 0),
                Capacity = 50,
                VenueId = 1
            };

            // Act
            var validationResults = ValidateModel(evt);

            // Assert
            validationResults.Should().ContainSingle(r => r.MemberNames.Contains("Name"));
        }

        [Fact]
        public void Event_WithNameTooLong_FailsValidation()
        {
            // Arrange
            var evt = new Event
            {
                Name = new string('A', 201), // Max is 200
                Description = "Description",
                EventDate = DateTime.UtcNow.AddDays(30),
                StartTime = new TimeSpan(9, 0, 0),
                Capacity = 50,
                VenueId = 1
            };

            // Act
            var validationResults = ValidateModel(evt);

            // Assert
            validationResults.Should().ContainSingle(r => r.MemberNames.Contains("Name"));
        }

        [Fact]
        public void Event_WithZeroCapacity_FailsValidation()
        {
            // Arrange
            var evt = new Event
            {
                Name = "Event",
                Description = "Description",
                EventDate = DateTime.UtcNow.AddDays(30),
                StartTime = new TimeSpan(9, 0, 0),
                Capacity = 0,
                VenueId = 1
            };

            // Act
            var validationResults = ValidateModel(evt);

            // Assert
            validationResults.Should().ContainSingle(r => r.MemberNames.Contains("Capacity"));
        }

        [Fact]
        public void Event_WithNegativeCapacity_FailsValidation()
        {
            // Arrange
            var evt = new Event
            {
                Name = "Event",
                Description = "Description",
                EventDate = DateTime.UtcNow.AddDays(30),
                StartTime = new TimeSpan(9, 0, 0),
                Capacity = -10,
                VenueId = 1
            };

            // Act
            var validationResults = ValidateModel(evt);

            // Assert
            validationResults.Should().ContainSingle(r => r.MemberNames.Contains("Capacity"));
        }

        #endregion

        #region Computed Properties Tests

        [Fact]
        public void RegisteredCount_ReturnsConfirmedRegistrationsOnly()
        {
            // Arrange
            var evt = new Event
            {
                Name = "Test Event",
                Description = "Description",
                EventDate = DateTime.UtcNow.AddDays(30),
                StartTime = new TimeSpan(9, 0, 0),
                Capacity = 100,
                VenueId = 1,
                Registrations = new List<Registration>
                {
                    new Registration { Status = RegistrationStatus.Confirmed },
                    new Registration { Status = RegistrationStatus.Confirmed },
                    new Registration { Status = RegistrationStatus.Cancelled },
                    new Registration { Status = RegistrationStatus.Waitlisted }
                }
            };

            // Act
            var count = evt.RegisteredCount;

            // Assert
            count.Should().Be(2);
        }

        [Fact]
        public void AvailableSpots_CalculatesCorrectly()
        {
            // Arrange
            var evt = new Event
            {
                Capacity = 100,
                Registrations = new List<Registration>
                {
                    new Registration { Status = RegistrationStatus.Confirmed },
                    new Registration { Status = RegistrationStatus.Confirmed },
                    new Registration { Status = RegistrationStatus.Confirmed }
                }
            };

            // Act
            var spots = evt.AvailableSpots;

            // Assert
            spots.Should().Be(97);
        }

        [Fact]
        public void IsFull_ReturnsTrue_WhenNoSpotsAvailable()
        {
            // Arrange
            var evt = new Event
            {
                Capacity = 2,
                Registrations = new List<Registration>
                {
                    new Registration { Status = RegistrationStatus.Confirmed },
                    new Registration { Status = RegistrationStatus.Confirmed }
                }
            };

            // Act
            var isFull = evt.IsFull;

            // Assert
            isFull.Should().BeTrue();
        }

        [Fact]
        public void IsFull_ReturnsFalse_WhenSpotsAvailable()
        {
            // Arrange
            var evt = new Event
            {
                Capacity = 10,
                Registrations = new List<Registration>
                {
                    new Registration { Status = RegistrationStatus.Confirmed }
                }
            };

            // Act
            var isFull = evt.IsFull;

            // Assert
            isFull.Should().BeFalse();
        }

        [Fact]
        public void IsUpcoming_ReturnsTrue_ForFutureUpcomingEvent()
        {
            // Arrange
            var evt = new Event
            {
                EventDate = DateTime.UtcNow.AddDays(30),
                Status = EventStatus.Upcoming
            };

            // Act
            var isUpcoming = evt.IsUpcoming;

            // Assert
            isUpcoming.Should().BeTrue();
        }

        [Fact]
        public void IsUpcoming_ReturnsFalse_ForPastEvent()
        {
            // Arrange
            var evt = new Event
            {
                EventDate = DateTime.UtcNow.AddDays(-1),
                Status = EventStatus.Upcoming
            };

            // Act
            var isUpcoming = evt.IsUpcoming;

            // Assert
            isUpcoming.Should().BeFalse();
        }

        [Fact]
        public void IsUpcoming_ReturnsFalse_ForCancelledEvent()
        {
            // Arrange
            var evt = new Event
            {
                EventDate = DateTime.UtcNow.AddDays(30),
                Status = EventStatus.Cancelled
            };

            // Act
            var isUpcoming = evt.IsUpcoming;

            // Assert
            isUpcoming.Should().BeFalse();
        }

        #endregion

        private static List<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
        }
    }
}
