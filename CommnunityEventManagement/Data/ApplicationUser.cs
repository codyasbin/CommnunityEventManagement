using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace CommnunityEventManagement.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsAdmin { get; set; } = false;

        // Navigation property for registrations
        public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
