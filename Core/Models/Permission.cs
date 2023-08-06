using System;
namespace Core.Models
{
    public class Permission
    {
        public int PermissionID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}

