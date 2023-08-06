using System;
using Microsoft.AspNetCore.Authorization;

namespace Core.Policy
{
    public static class PermissionsPolicy
    {
        public const string Read = "Read";
        public const string Write = "Write";
        public const string Delete = "Delete";
        public const string Update = "Update";
    }

    public static class AuthorizationPolicies
    {
        public static AuthorizeAttribute ReadPermission()
        {
            return new AuthorizeAttribute { Policy = PermissionsPolicy.Read };
        }

        public static AuthorizeAttribute WritePermission()
        {
            return new AuthorizeAttribute { Policy = PermissionsPolicy.Write };
        }

        public static AuthorizeAttribute DeletePermission()
        {
            return new AuthorizeAttribute { Policy = PermissionsPolicy.Delete };
        }

        public static AuthorizeAttribute UpdatePermission()
        {
            return new AuthorizeAttribute { Policy = PermissionsPolicy.Update };
        }
    }
}

