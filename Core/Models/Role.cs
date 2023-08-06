using System;
namespace Core.Models
{
    public class Role
    {
        public int RoleID { get; set; }
        public string Name { get; set; } = string.Empty;
        public IEnumerable<Permission> Permissions { get; set; } = Enumerable.Empty<Permission>();
    }
}

