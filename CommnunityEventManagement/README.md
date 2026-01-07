# Community Event Management System (CEMS)

A modern web application built with **Blazor .NET 10** for managing community events, registrations, and activities.

![.NET 10](https://img.shields.io/badge/.NET-10.0-purple)
![Blazor](https://img.shields.io/badge/Blazor-Server-blue)
![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL-336791)
![xUnit](https://img.shields.io/badge/Tests-xUnit-green)
![License](https://img.shields.io/badge/License-MIT-green)

## &#x1F4CB; Features

### For Participants
- &#x1F50D; **Browse Events** - Discover community events with advanced filtering (date, venue, activity, search)
- &#x1F4DD; **Event Registration** - Register for events with real-time capacity tracking
- &#x1F3AB; **My Registrations** - View and manage your event registrations
- &#x274C; **Cancel Registration** - Cancel registrations when plans change

### For Administrators
- &#x1F4C5; **Event Management** - Create, edit, and delete events
- &#x1F3E2; **Venue Management** - Manage event locations and capacities
- &#x1F3AF; **Activity Management** - Organize activities by categories
- &#x1F465; **Registration Overview** - View all registrations across the system

### Authentication & Security
- &#x1F512; Role-based access control (Admin, Participant)
- &#x1F511; ASP.NET Core Identity integration
- &#x1F6E1; Secure authentication with cookies

## &#x1F680; Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL](https://www.postgresql.org/download/) (or SQL Server - see configuration below)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/codyasbin/CommnunityEventManagement.git
   cd CommnunityEventManagement
   ```

2. **Configure the database connection**
   
   Update the connection string in `appsettings.json`:
   
   **For PostgreSQL:**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=cems_db;Username=your_username;Password=your_password"
     }
   }
   ```
   
   **For SQL Server:**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CEMS_DB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
     }
   }
   ```

3. **Apply database migrations**
   ```bash
   cd CommnunityEventManagement
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Open in browser**
   - HTTPS: `https://localhost:7145`
   - HTTP: `http://localhost:5286`

### Default Admin Credentials

| Email | Password |
|-------|----------|
| admin@cems.com | Admin@123 |

> &#x26A0; **Important**: Change the default admin password in production!

## &#x1F4C1; Project Structure

```
CommnunityEventManagement/
??? CommnunityEventManagement/          # Main Blazor Application
?   ??? Components/
?   ?   ??? Account/                    # Identity/Authentication pages
?   ?   ??? Layout/                     # AdminLayout, ParticipantLayout
?   ?   ??? Pages/
?   ?       ??? Admin/                  # Admin management pages
?   ?       ?   ??? Activities.razor
?   ?       ?   ??? Dashboard.razor
?   ?       ?   ??? Events.razor
?   ?       ?   ??? Registrations.razor
?   ?       ?   ??? Venues.razor
?   ?       ??? Events.razor            # Browse events
?   ?       ??? EventDetails.razor      # Event details & registration
?   ?       ??? Home.razor              # Landing page
?   ?       ??? MyRegistrations.razor   # User registrations
?   ??? Data/
?   ?   ??? Models/                     # Entity models
?   ?   ??? ApplicationDbContext.cs
?   ??? Services/                       # Business logic services
?   ?   ??? ActivityService.cs
?   ?   ??? EventService.cs
?   ?   ??? RegistrationService.cs
?   ?   ??? VenueService.cs
?   ??? wwwroot/
?       ??? css/                        # Stylesheets
?
??? CommnunityEventManagement.Tests/    # Unit Test Project
    ??? Helpers/
    ?   ??? TestDbContextFactory.cs     # In-memory DB helper
    ??? Models/                         # Model validation tests
    ?   ??? EventModelTests.cs
    ?   ??? RegistrationModelTests.cs
    ?   ??? VenueModelTests.cs
    ??? Services/                       # Service layer tests
        ??? ActivityServiceTests.cs
        ??? EventServiceTests.cs
        ??? RegistrationServiceTests.cs
        ??? VenueServiceTests.cs
```

## &#x1F9EA; Testing

The project includes a comprehensive unit test suite using **xUnit**, **FluentAssertions**, and **Moq**.

### Test Project Structure

| Test Category | Test File | Tests | Description |
|---------------|-----------|-------|-------------|
| **Services** | EventServiceTests.cs | 18 | CRUD, filtering, upcoming events, registration checks |
| **Services** | RegistrationServiceTests.cs | 16 | CRUD, user registrations, cancellation, validation |
| **Services** | VenueServiceTests.cs | 10 | CRUD operations, ordering |
| **Services** | ActivityServiceTests.cs | 10 | CRUD operations, category filtering |
| **Models** | EventModelTests.cs | 12 | Validation, computed properties (IsFull, IsUpcoming) |
| **Models** | RegistrationModelTests.cs | 6 | Validation, default values, status enum |
| **Models** | VenueModelTests.cs | 9 | Validation, default values |

**Total: 81 Unit Tests**

### Running Tests

#### Using .NET CLI

```bash
# Navigate to test project
cd CommnunityEventManagement.Tests

# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run specific test class
dotnet test --filter "FullyQualifiedName~EventServiceTests"

# Run tests with code coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate coverage report (requires reportgenerator tool)
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

#### Using Visual Studio

1. Open **Test Explorer** (Test ? Test Explorer or Ctrl+E, T)
2. Click **Run All** to execute all tests
3. View test results and code coverage in the Test Explorer window

#### Using Visual Studio Code

1. Install the **.NET Core Test Explorer** extension
2. Open the Testing sidebar
3. Click the play button to run tests

### Test Coverage Areas

- &#x2705; **Event Service**: Create, Read, Update, Delete events; filtering by date, venue, activity; upcoming events logic
- &#x2705; **Registration Service**: User registration, cancellation, duplicate prevention, capacity checks
- &#x2705; **Venue Service**: CRUD operations, event associations
- &#x2705; **Activity Service**: CRUD operations, category management
- &#x2705; **Model Validation**: Required fields, string lengths, range validation
- &#x2705; **Computed Properties**: RegisteredCount, AvailableSpots, IsFull, IsUpcoming

### Test Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| xUnit | 2.9.2 | Test framework |
| xUnit.runner.visualstudio | 2.8.2 | VS Test Explorer integration |
| FluentAssertions | 6.12.2 | Readable assertions |
| Moq | 4.20.72 | Mocking framework |
| Microsoft.EntityFrameworkCore.InMemory | 9.0.0 | In-memory database for testing |
| coverlet.collector | 6.0.2 | Code coverage collection |

## &#x1F5C3; Database Schema

```
???????????????     ???????????????????     ???????????????
?   Venues    ?     ?     Events      ?     ? Activities  ?
???????????????     ???????????????????     ???????????????
? Id          ??????? VenueId         ?     ? Id          ?
? Name        ?     ? Id              ?     ? Name        ?
? Address     ?     ? Name            ?     ? Description ?
? MaxCapacity ?     ? Description     ?     ? Category    ?
? Description ?     ? EventDate       ?     ???????????????
???????????????     ? StartTime       ?            ?
                    ? EndTime         ?            ?
                    ? Capacity        ?     ???????????????
                    ? Status          ?     ?EventActivity?
                    ???????????????????     ???????????????
                             ?              ? EventId     ?
                             ?              ? ActivityId  ?
                    ???????????????????     ???????????????
                    ?  Registrations  ?
                    ???????????????????
                    ? Id              ?
                    ? UserId          ?????? ApplicationUser
                    ? EventId         ?
                    ? Status          ?
                    ? RegistrationDate?
                    ???????????????????
```

## &#x1F3A8; UI Features

- **Modern Design** - Gradient-based color scheme with card layouts
- **Dual Layouts** - Admin dashboard layout & Participant website layout
- **Responsive** - Works on desktop and mobile devices
- **Real-time Updates** - Blazor Server for instant UI updates
- **Capacity Indicators** - Visual progress bars showing event capacity
- **Status Badges** - Color-coded status for events and registrations

## &#x1F527; Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | Development |
| `ConnectionStrings__DefaultConnection` | Database connection | See appsettings.json |

### App Settings

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## &#x1F6E0; Development

### Running in Development Mode

```bash
dotnet watch run
```

### Creating New Migrations

```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

### Building for Production

```bash
dotnet publish -c Release -o ./publish
```

### Running All Tests Before Commit

```bash
# From solution root
dotnet test CommnunityEventManagement.Tests/CommnunityEventManagement.Tests.csproj
```

## &#x1F91D; Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Write tests for your changes
4. Ensure all tests pass (`dotnet test`)
5. Commit your changes (`git commit -m 'Add amazing feature'`)
6. Push to the branch (`git push origin feature/amazing-feature`)
7. Open a Pull Request

### Contribution Guidelines

- &#x2705; All new features must include unit tests
- &#x2705; All tests must pass before merging
- &#x2705; Follow existing code style and patterns
- &#x2705; Update documentation for significant changes

## &#x1F4C4; License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## &#x1F465; Authors

- **Asbin** - *Initial work* - [codyasbin](https://github.com/codyasbin)

## &#x1F64F; Acknowledgments

- ASP.NET Core team for the amazing Blazor framework
- Bootstrap for the responsive UI components
- xUnit team for the excellent testing framework
- FluentAssertions for readable test assertions

---

Made with &#x2764; using Blazor .NET 10
