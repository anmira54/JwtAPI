using System;
namespace Core.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int RoleID { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime LastUpdated { get; set; }
        public Role Role { get; set; } = new Role();
    }
}

