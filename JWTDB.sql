IF EXISTS (SELECT name FROM sys.databases WHERE name = N'JWTDB')
DROP DATABASE JWTDB;

CREATE DATABASE JWTDB;
GO

USE JWTDB;
GO

CREATE TABLE Permissions (
    PermissionID INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(50),
    Description NVARCHAR(255)
);

CREATE TABLE PermissionGroups (
    GroupID INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(50)
);

GO

CREATE TABLE PermissionGroupDetails (
    GroupID INT FOREIGN KEY REFERENCES PermissionGroups(GroupID),
    PermissionID INT FOREIGN KEY REFERENCES Permissions(PermissionID),
    PRIMARY KEY (GroupID, PermissionID)
);

CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(50)
);

GO

CREATE TABLE RolePermissionGroups (
    RoleID INT FOREIGN KEY REFERENCES Roles(RoleID),
    GroupID INT FOREIGN KEY REFERENCES PermissionGroups(GroupID),
    PRIMARY KEY (RoleID, GroupID)
);

CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY,
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(60) NOT NULL, 
    RoleID INT FOREIGN KEY REFERENCES Roles(RoleID),
    rowguid UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    LastUpdated DATETIME NOT NULL DEFAULT GETDATE()
);

GO

CREATE TRIGGER trg_UpdateLastUpdated
ON Users
AFTER UPDATE
AS
BEGIN
    UPDATE Users
    SET LastUpdated = GETDATE()
    FROM Users u
    JOIN inserted i ON u.UserID = i.UserID
END;

GO

INSERT INTO Roles (Name) VALUES ('Admin');
INSERT INTO Roles (Name) VALUES ('Editor');
INSERT INTO Roles (Name) VALUES ('Viewer');

GO

INSERT INTO Permissions (Name, Description) VALUES ('Read', 'Permiso para leer');
INSERT INTO Permissions (Name, Description) VALUES ('Write', 'Permiso para escribir');
INSERT INTO Permissions (Name, Description) VALUES ('Delete', 'Permiso para eliminar');

GO

INSERT INTO PermissionGroups (Name) VALUES ('ReadWrite');
INSERT INTO PermissionGroupDetails (GroupID, PermissionID) VALUES (1, 1); -- Grupo ReadWrite: permiso Read
INSERT INTO PermissionGroupDetails (GroupID, PermissionID) VALUES (1, 2); -- Grupo ReadWrite: permiso Write

GO

INSERT INTO PermissionGroups (Name) VALUES ('Read');
INSERT INTO PermissionGroupDetails (GroupID, PermissionID) VALUES (2, 1); -- Grupo Read: permiso Read

GO

INSERT INTO RolePermissionGroups (RoleID, GroupID) VALUES (1, 1); -- Admin: Grupo ReadWrite
INSERT INTO RolePermissionGroups (RoleID, GroupID) VALUES (2, 2); -- Editor: Grupo Read
INSERT INTO RolePermissionGroups (RoleID, GroupID) VALUES (3, 2); -- Viewer: Grupo Read

GO

INSERT INTO Users (Username, PasswordHash, RoleID) VALUES ('admin_user', '$2a$08$nccCvfOy09iYAiuYMxz0k.6D9uAK52bFIUhtHUIEovXLGjdKt.cjK', 1); -- Role: Admin - Pasword: CPJ-ryn6dab@dtp9utb
INSERT INTO Users (Username, PasswordHash, RoleID) VALUES ('editor_user1', '$2a$08$DfGs1LYGxDwEmioI7z0NTOXb44/FBDg3efbpWE3nCYnukS2I3xM0K', 2); -- Role: Editor - Password: ckr.xkr6guv1PWZ-adv
INSERT INTO Users (Username, PasswordHash, RoleID) VALUES ('editor_user2', '$2a$08$RKNnAu7Eixl1XEVrdWTkwOhXrisNIze0YCYhoAe7JnwBooPh4dg1y', 2); -- Role: Editor - Password: bqe2dzm@TBF4gfh6cwb
INSERT INTO Users (Username, PasswordHash, RoleID) VALUES ('viewer_user1', '$2a$08$yMHkmCDoLZyN89x.48HFn.jZGp64RhWrMUYlH7MJTwkuJxddFT0u.', 3); -- Role: Viewer - Password: EWK5pay*xuv1myt1zgx
INSERT INTO Users (Username, PasswordHash, RoleID) VALUES ('viewer_user2', '$2a$08$HyUkUHxnUgl7a.uK.ehZ0ugj4MNdyJ00o67ijbYv15tz5hr1CgYRa', 3); -- Role: Viewer - Password: KRJ_gzm4rpd-rxt8txm