using System;
using System.Data.SqlClient;
using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Options;

namespace Core.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppSettings _appSettings;
        public UserRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public User GetUser(string username)
        {
            using SqlConnection connection = new(_appSettings.ConnectionString);
            connection.Open();

            string query = @"
                                SELECT TOP 1 U.* FROM Users U WHERE U.Username = @UserName
                            ";
            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@UserName", username);

            using SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                int roleId = reader.GetInt32(reader.GetOrdinal("RoleID"));
                User user = new()
                {
                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                    UserName = reader.GetString(reader.GetOrdinal("Username")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                    RoleID = roleId,
                    Rowguid = reader.GetGuid(reader.GetOrdinal("rowguid")),
                    LastUpdated = reader.GetDateTime(reader.GetOrdinal("LastUpdated")),
                    Role = GetRole(roleId)
                };

                return user;
            }

            throw new InvalidOperationException($"No se encontró el nombre de usuario {username}");

        }

        public Role GetRole(int roleId)
        {
            using SqlConnection connection = new(_appSettings.ConnectionString);
            connection.Open();

            string query = @"
                                SELECT R.* FROM Roles R WHERE R.RoleID = @RoleId
                            ";

            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@RoleId", roleId);

            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                Role role = new()
                {
                    RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Permissions = GetRolePermissions(roleId)
                };

                return role;
            }

            throw new InvalidOperationException($"No se encontró el rol {roleId}");
        }

        public IEnumerable<Permission> GetRolePermissions(int roleId)
        {
            using SqlConnection connection = new(_appSettings.ConnectionString);
            connection.Open();

            string query = @"
                                SELECT P.* FROM RolePermissionGroups RPG
                                INNER JOIN PermissionGroupDetails PGD ON PGD.GroupID = RPG.GroupID
                                INNER JOIN Permissions P ON P.PermissionID = PGD.PermissionID
                                WHERE RPG.RoleID = @RoleId
                            ";

            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@RoleId", roleId);

            using SqlDataReader reader = command.ExecuteReader();
            List<Permission> permissions = new();

            while (reader.Read())
            {
                Permission permission = new()
                {
                    PermissionID = reader.GetInt32(reader.GetOrdinal("PermissionID")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Descripcion = reader.GetString(reader.GetOrdinal("Description"))
                };

                permissions.Add(permission);
            }

            return permissions;
        }
    }
}