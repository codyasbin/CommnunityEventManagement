# Community Event Management System (CEMS)

A modern web application built with **Blazor .NET 10** for managing community events, registrations, and activities.

![.NET 10](https://img.shields.io/badge/.NET-10.0-purple)
![Blazor](https://img.shields.io/badge/Blazor-Server-blue)
![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL-336791)
![License](https://img.shields.io/badge/License-MIT-green)

## ?? Features

### For Participants
- ?? **Browse Events** - Discover community events with advanced filtering (date, venue, activity, search)
- ?? **Event Registration** - Register for events with real-time capacity tracking
- ?? **My Registrations** - View and manage your event registrations
- ? **Cancel Registration** - Cancel registrations when plans change

### For Administrators
- ?? **Event Management** - Create, edit, and delete events
- ?? **Venue Management** - Manage event locations and capacities
- ?? **Activity Management** - Organize activities by categories
- ?? **Registration Overview** - View all registrations across the system

### Authentication & Security
- ?? Role-based access control (Admin, Participant)
- ?? ASP.NET Core Identity integration
- ??? Secure authentication with cookies

## ?? Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL](https://www.postgresql.org/download/) (or update connection string for your database)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/codyasbin/CommnunityEventManagement.git
   cd CommnunityEventManagement
   ```

2. **Configure the database connection**
   
   Update the connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=cems_db;Username=your_username;Password=your_password"
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

> ?? **Important**: Change the default admin password in production!

## ?? Project Structure

```
CommnunityEventManagement/
??? Components/
?   ??? Account/           # Identity/Authentication pages
?   ??? Layout/            # MainLayout, NavMenu
?   ??? Pages/
?       ??? Admin/         # Admin management pages
?       ?   ??? Activities.razor
?       ?   ??? Events.razor
?       ?   ??? Registrations.razor
?       ?   ??? Venues.razor
?       ??? Events.razor           # Browse events
?       ??? EventDetails.razor     # Event details & registration
?       ??? Home.razor             # Landing page
?       ??? MyRegistrations.razor  # User registrations
??? Data/
?   ??? Models/            # Entity models
?   ?   ??? Activity.cs
?   ?   ??? Event.cs
?   ?   ??? EventActivity.cs
?   ?   ??? Registration.cs
?   ?   ??? Venue.cs
?   ??? ApplicationDbContext.cs
??? Services/              # Business logic services
?   ??? ActivityService.cs
?   ??? EventService.cs
?   ??? RegistrationService.cs
?   ??? VenueService.cs
??? wwwroot/
    ??? app.css            # Custom styles
```

## ??? Database Schema

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

## ?? UI Features

- **Modern Design** - Gradient-based color scheme with card layouts
- **Responsive** - Works on desktop and mobile devices
- **Real-time Updates** - Blazor Server for instant UI updates
- **Capacity Indicators** - Visual progress bars showing event capacity
- **Status Badges** - Color-coded status for events and registrations

## ?? Configuration

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

## ?? API Endpoints

The application uses Blazor Server with SignalR for real-time communication. Additional REST endpoints:

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /Account/Login | User login |
| POST | /Account/Logout | User logout |
| POST | /Account/Register | New user registration |

## ?? Development

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

## ?? Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## ?? License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## ?? Authors

- **Asbin** - *Initial work* - [codyasbin](https://github.com/codyasbin)

## ?? Acknowledgments

- ASP.NET Core team for the amazing Blazor framework
- Bootstrap for the responsive UI components
- PostgreSQL for the robust database system

---

Made with ?? using Blazor .NET 10
