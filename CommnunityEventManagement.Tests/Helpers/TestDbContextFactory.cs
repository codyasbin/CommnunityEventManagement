using CommnunityEventManagement.Data;
using CommnunityEventManagement.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace CommnunityEventManagement.Tests.Helpers
{
    public static class TestDbContextFactory
    {
        public static ApplicationDbContext CreateInMemoryContext(string? databaseName = null)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        public static async Task<ApplicationDbContext> CreateSeededContextAsync(string? databaseName = null)
        {
            var context = CreateInMemoryContext(databaseName);
            await SeedTestDataAsync(context);
            return context;
        }

        public static async Task SeedTestDataAsync(ApplicationDbContext context)
        {
            // Seed Venues
            var venues = new List<Venue>
            {
                new Venue
                {
                    Id = 1,
                    Name = "Main Hall",
                    Address = "123 Main Street",
                    MaxCapacity = 500,
                    Description = "Large event hall",
                    CreatedAt = DateTime.UtcNow
                },
                new Venue
                {
                    Id = 2,
                    Name = "Conference Room A",
                    Address = "456 Business Ave",
                    MaxCapacity = 50,
                    Description = "Small meeting room",
                    CreatedAt = DateTime.UtcNow
                }
            };

            // Seed Activities
            var activities = new List<Activity>
            {
                new Activity
                {
                    Id = 1,
                    Name = "Workshop",
                    Category = "Education",
                    Description = "Hands-on learning session",
                    CreatedAt = DateTime.UtcNow
                },
                new Activity
                {
                    Id = 2,
                    Name = "Networking",
                    Category = "Social",
                    Description = "Meet and greet session",
                    CreatedAt = DateTime.UtcNow
                },
                new Activity
                {
                    Id = 3,
                    Name = "Presentation",
                    Category = "Education",
                    Description = "Keynote presentation",
                    CreatedAt = DateTime.UtcNow
                }
            };

            // Seed Users
            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = "user-1",
                    UserName = "john@test.com",
                    Email = "john@test.com",
                    FirstName = "John",
                    LastName = "Doe",
                    EmailConfirmed = true
                },
                new ApplicationUser
                {
                    Id = "user-2",
                    UserName = "jane@test.com",
                    Email = "jane@test.com",
                    FirstName = "Jane",
                    LastName = "Smith",
                    EmailConfirmed = true
                },
                new ApplicationUser
                {
                    Id = "admin-1",
                    UserName = "admin@test.com",
                    Email = "admin@test.com",
                    FirstName = "Admin",
                    LastName = "User",
                    IsAdmin = true,
                    EmailConfirmed = true
                }
            };

            // Seed Events
            var events = new List<Event>
            {
                new Event
                {
                    Id = 1,
                    Name = "Tech Conference 2025",
                    Description = "Annual technology conference",
                    EventDate = DateTime.UtcNow.AddDays(30),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    Capacity = 100,
                    Status = EventStatus.Upcoming,
                    VenueId = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new Event
                {
                    Id = 2,
                    Name = "Networking Mixer",
                    Description = "Professional networking event",
                    EventDate = DateTime.UtcNow.AddDays(7),
                    StartTime = new TimeSpan(18, 0, 0),
                    EndTime = new TimeSpan(21, 0, 0),
                    Capacity = 30,
                    Status = EventStatus.Upcoming,
                    VenueId = 2,
                    CreatedAt = DateTime.UtcNow
                },
                new Event
                {
                    Id = 3,
                    Name = "Past Event",
                    Description = "This event already happened",
                    EventDate = DateTime.UtcNow.AddDays(-10),
                    StartTime = new TimeSpan(10, 0, 0),
                    Capacity = 50,
                    Status = EventStatus.Completed,
                    VenueId = 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                },
                new Event
                {
                    Id = 4,
                    Name = "Full Event",
                    Description = "This event is at capacity",
                    EventDate = DateTime.UtcNow.AddDays(14),
                    StartTime = new TimeSpan(14, 0, 0),
                    Capacity = 2,
                    Status = EventStatus.Upcoming,
                    VenueId = 2,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.Venues.AddRangeAsync(venues);
            await context.Activities.AddRangeAsync(activities);
            await context.Users.AddRangeAsync(users);
            await context.Events.AddRangeAsync(events);

            // Seed EventActivities
            var eventActivities = new List<EventActivity>
            {
                new EventActivity { EventId = 1, ActivityId = 1 },
                new EventActivity { EventId = 1, ActivityId = 3 },
                new EventActivity { EventId = 2, ActivityId = 2 }
            };
            await context.Set<EventActivity>().AddRangeAsync(eventActivities);

            // Seed Registrations for "Full Event"
            var registrations = new List<Registration>
            {
                new Registration
                {
                    Id = 1,
                    UserId = "user-1",
                    EventId = 4,
                    RegistrationDate = DateTime.UtcNow.AddDays(-1),
                    Status = RegistrationStatus.Confirmed
                },
                new Registration
                {
                    Id = 2,
                    UserId = "user-2",
                    EventId = 4,
                    RegistrationDate = DateTime.UtcNow.AddDays(-1),
                    Status = RegistrationStatus.Confirmed
                }
            };
            await context.Registrations.AddRangeAsync(registrations);

            await context.SaveChangesAsync();
        }
    }
}
