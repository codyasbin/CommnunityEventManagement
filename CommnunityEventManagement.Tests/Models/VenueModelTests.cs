using CommnunityEventManagement.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace CommnunityEventManagement.Tests.Models
{
    public class VenueModelTests
    {
        #region Validation Tests

        [Fact]
        public void Venue_WithValidData_PassesValidation()
        {
            // Arrange
            var venue = new Venue
            {
                Name = "Main Hall",
                Address = "123 Main Street",
                MaxCapacity = 500
            };

            // Act
            var validationResults = ValidateModel(venue);

            // Assert
            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void Venue_WithEmptyName_FailsValidation()
        {
            // Arrange
            var venue = new Venue
            {
                Name = "",
                Address = "123 Main Street",
                MaxCapacity = 500
            };

            // Act
            var validationResults = ValidateModel(venue);

            // Assert
            validationResults.Should().ContainSingle(r => r.MemberNames.Contains("Name"));
        }

        [Fact]
        public void Venue_WithNameTooLong_FailsValidation()
        {
            // Arrange
            var venue = new Venue
            {
                Name = new string('A', 201), // Max is 200
                Address = "123 Main Street",
                MaxCapacity = 500
            };

            // Act
            var validationResults = ValidateModel(venue);

            // Assert
            validationResults.Should().ContainSingle(r => r.MemberNames.Contains("Name"));
        }

        [Fact]
        public void Venue_WithEmptyAddress_FailsValidation()
        {
            // Arrange
            var venue = new Venue
            {
                Name = "Main Hall",
                Address = "",
                MaxCapacity = 500
            };

            // Act
            var validationResults = ValidateModel(venue);

            // Assert
            validationResults.Should().ContainSingle(r => r.MemberNames.Contains("Address"));
        }

        [Fact]
        public void Venue_WithZeroCapacity_FailsValidation()
        {
            // Arrange
            var venue = new Venue
            {
                Name = "Main Hall",
                Address = "123 Main Street",
                MaxCapacity = 0
            };

            // Act
            var validationResults = ValidateModel(venue);

            // Assert
            validationResults.Should().ContainSingle(r => r.MemberNames.Contains("MaxCapacity"));
        }

        [Fact]
        public void Venue_WithNegativeCapacity_FailsValidation()
        {
            // Arrange
            var venue = new Venue
            {
                Name = "Main Hall",
                Address = "123 Main Street",
                MaxCapacity = -100
            };

            // Act
            var validationResults = ValidateModel(venue);

            // Assert
            validationResults.Should().ContainSingle(r => r.MemberNames.Contains("MaxCapacity"));
        }

        #endregion

        #region Default Values Tests

        [Fact]
        public void Venue_HasDefaultCreatedAtOfUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow;
            
            // Act
            var venue = new Venue();
            
            // Assert
            venue.CreatedAt.Should().BeOnOrAfter(before);
            venue.CreatedAt.Should().BeOnOrBefore(DateTime.UtcNow);
        }

        [Fact]
        public void Venue_HasEmptyEventsCollection_ByDefault()
        {
            // Arrange & Act
            var venue = new Venue();

            // Assert
            venue.Events.Should().NotBeNull();
            venue.Events.Should().BeEmpty();
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
