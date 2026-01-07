using CommnunityEventManagement.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace CommnunityEventManagement.Tests.Models
{
    public class RegistrationModelTests
    {
        #region Validation Tests

        [Fact]
        public void Registration_WithValidData_PassesValidation()
        {
            // Arrange
            var registration = new Registration
            {
                UserId = "user-123",
                EventId = 1,
                Status = RegistrationStatus.Confirmed
            };

            // Act
            var validationResults = ValidateModel(registration);

            // Assert
            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void Registration_WithEmptyUserId_FailsValidation()
        {
            // Arrange
            var registration = new Registration
            {
                UserId = "",
                EventId = 1,
                Status = RegistrationStatus.Confirmed
            };

            // Act
            var validationResults = ValidateModel(registration);

            // Assert
            validationResults.Should().ContainSingle(r => r.MemberNames.Contains("UserId"));
        }

        [Fact]
        public void Registration_WithNotesTooLong_FailsValidation()
        {
            // Arrange
            var registration = new Registration
            {
                UserId = "user-123",
                EventId = 1,
                Notes = new string('A', 501) // Max is 500
            };

            // Act
            var validationResults = ValidateModel(registration);

            // Assert
            validationResults.Should().ContainSingle(r => r.MemberNames.Contains("Notes"));
        }

        #endregion

        #region Default Values Tests

        [Fact]
        public void Registration_HasDefaultStatusOfConfirmed()
        {
            // Arrange & Act
            var registration = new Registration();

            // Assert
            registration.Status.Should().Be(RegistrationStatus.Confirmed);
        }

        [Fact]
        public void Registration_HasDefaultRegistrationDateOfUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow;
            
            // Act
            var registration = new Registration();
            
            // Assert
            registration.RegistrationDate.Should().BeOnOrAfter(before);
            registration.RegistrationDate.Should().BeOnOrBefore(DateTime.UtcNow);
        }

        [Fact]
        public void Registration_CancelledAtIsNull_ByDefault()
        {
            // Arrange & Act
            var registration = new Registration();

            // Assert
            registration.CancelledAt.Should().BeNull();
        }

        #endregion

        #region Status Enum Tests

        [Theory]
        [InlineData(RegistrationStatus.Confirmed)]
        [InlineData(RegistrationStatus.Cancelled)]
        [InlineData(RegistrationStatus.Waitlisted)]
        public void RegistrationStatus_HasExpectedValues(RegistrationStatus status)
        {
            // Assert - just verifying the enum values exist
            status.Should().BeDefined();
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
