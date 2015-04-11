// Copyright (c) KHNURE, Inc. All rights reserved.

using InterSystems.Data.CacheClient;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    class IdentityDbInitializer : CreateDatabaseIfNotExists<IdentityDbContext>
    {
        #region Constants

        private const string UsersQuery = "SELECT * FROM DBO.AspNetUsers";
        private const string UserRolesQuery = "SELECT * FROM DBO.AspNetUserRoles";
        private const string RolesQuery = "SELECT * FROM DBO.AspNetRoles";
        private const string UserClaimsQuery = "SELECT * FROM DBO.AspNetUserClaims";
        private const string UserLoginsQuery = "SELECT * FROM DBO.AspNetUserLogins";

        private const string AspNetUsers = "DBO.AspNetUsers";
        private const string AspNetRoles = "DBO.AspNetRoles";
        private const string AspNetUserRoles = "DBO.AspNetUserRoles";
        private const string AspNetUserClaims = "DBO.AspNetUserClaims";
        private const string AspNetUserLogins = "DBO.AspNetUserLogins";

        private const string ExistingTablesQuery = "SELECT id FROM %Dictionary.CompiledClass WHERE SqlTableName='AspNetUsers' OR SqlTableName='AspNetRoles' OR SqlTableName='AspNetUserRoles' OR SqlTableName='AspNetUserClaims' OR SqlTableName='AspNetUserLogins'";

        #endregion

        #region Helpers

        private string GetTableQuery(string name)
        {
            switch (name)
            {
                case AspNetUsers:
                    return "CREATE TABLE DBO.AspNetUsers (Id nvarchar(128) NOT NULL PRIMARY KEY, Email nvarchar(256), EmailConfirmed bit NOT NULL, PasswordHash nvarchar(MAX), SecurityStamp nvarchar(MAX), PhoneNumber nvarchar(MAX), PhoneNumberConfirmed bit NOT NULL, TwoFactorEnabled bit NOT NULL, LockoutEndDateUtc datetime, LockoutEnabled bit NOT NULL, AccessFailedCount int NOT NULL, UserName nvarchar(256) NOT NULL UNIQUE)";
                case AspNetRoles:
                    return "CREATE TABLE DBO.AspNetRoles (Id nvarchar(128) NOT NULL PRIMARY KEY, Name nvarchar(256))";
                case AspNetUserRoles:
                    return "CREATE TABLE DBO.AspNetUserRoles (UserId nvarchar(128) NOT NULL, RoleId nvarchar(128), CONSTRAINT PK_AspNetUserRoles PRIMARY KEY CLUSTERED (UserId), FOREIGN KEY (UserId) REFERENCES DBO.AspNetUsers(Id), FOREIGN KEY (RoleId) REFERENCES DBO.AspNetRoles(Id))";
                case AspNetUserClaims:
                    return "CREATE TABLE DBO.AspNetUserClaims (Id int NOT NULL, UserId nvarchar(128) NOT NULL, ClaimType nvarchar(MAX), ClaimValue nvarchar(MAX), CONSTRAINT PK_AspNetUserClaims PRIMARY KEY CLUSTERED (Id), FOREIGN KEY (UserId) REFERENCES DBO.AspNetUsers(Id))";
                case AspNetUserLogins:
                    return "CREATE TABLE DBO.AspNetUserLogins (LoginProvider NVARCHAR(128) NOT NULL, ProviderKey NVARCHAR(128) NOT NULL, UserId NVARCHAR(128) NOT NULL, CONSTRAINT PK_AspNetUserLogins PRIMARY KEY CLUSTERED (LoginProvider, ProviderKey, UserId), FOREIGN KEY (UserId) REFERENCES DBO.AspNetUsers(Id))";
                default:
                    throw new ArgumentException("Unexpected table name!");
            }
        }

        private void CreateTableIfNotExists(List<string> tables, string tableName, CacheConnection connection)
        {
            if (tables.Count(x => x == tableName) == 0)
                using (var command = new CacheCommand(GetTableQuery(tableName), connection))
                    command.ExecuteScalar();
        }

        private CacheConnection BuildConnection(IdentityDbContext context)
        {
            var connection = new CacheConnection();
            connection.ConnectionString = context.Database.Connection.ConnectionString;
            connection.Open();

            return connection;
        }

        private IdentityUser GetUser(IdentityDbContext context, string id)
        {
            return context.Users.FirstOrDefault(x => x.Id == id);
        }

        private void CheckNotNull(Object obj)
        {
            if (obj == null)
                throw new ArgumentException("Object does not exist!");
        }

        #endregion

        #region InitializeComponent

        private List<string> GetExistingTables(CacheConnection connection)
        {
            var tables = new List<string>();
            using (var command = new CacheCommand(ExistingTablesQuery, connection))
            using (var tablesReader = command.ExecuteReader())
            {
                while (tablesReader.Read())
                    tables.Add(tablesReader[0].ToString());
            }

            return tables;
        }

        public override void InitializeDatabase(IdentityDbContext context)
        {
            using (var connection = BuildConnection(context))
            {
                var tables = GetExistingTables(connection);

                CreateTableIfNotExists(tables, AspNetUsers, connection);
                CreateTableIfNotExists(tables, AspNetRoles, connection);
                CreateTableIfNotExists(tables, AspNetUserRoles, connection);
                CreateTableIfNotExists(tables, AspNetUserClaims, connection);
                CreateTableIfNotExists(tables, AspNetUserLogins, connection);
            }
        }

        private void InitUsers(CacheDataReader usersReader, IdentityDbContext context)
        {
            while (usersReader.Read())
            {
                var user = new IdentityUser();
                user.AccessFailedCount = (int)usersReader["AccessFailedCount"];
                user.Email = usersReader["Email"] as String;
                user.EmailConfirmed = (bool)usersReader["EmailConfirmed"];
                user.Id = (string)usersReader["Id"];
                user.LockoutEnabled = (bool)usersReader["LockoutEnabled"];
                user.LockoutEndDateUtc = usersReader["LockoutEndDateUtc"] as DateTime?;
                user.PasswordHash = usersReader["PasswordHash"] as String;
                user.PhoneNumber = usersReader["PhoneNumber"] as String;
                user.PhoneNumberConfirmed = (bool)usersReader["PhoneNumberConfirmed"];
                user.SecurityStamp = usersReader["SecurityStamp"] as String;
                user.TwoFactorEnabled = (bool)usersReader["TwoFactorEnabled"];
                user.UserName = (string)usersReader["UserName"];
                context.Users.Add(user);
            }
        }

        private void InitRoles(CacheDataReader rolesReader, IdentityDbContext context)
        {
            while (rolesReader.Read())
            {
                var role = new IdentityRole();
                role.Id = (string)rolesReader["Id"];
                role.Name = (string)rolesReader["Name"];
                context.Roles.Add(role);
            }
        }

        private void InitUserRoles(CacheDataReader userRolesReader, IdentityDbContext context)
        {
            while (userRolesReader.Read())
            {
                var userRole = new IdentityUserRole();
                userRole.UserId = (string)userRolesReader["UserId"];
                userRole.RoleId = (string)userRolesReader["RoleId"];

                var user = GetUser(context, userRole.UserId);
                var role = context.Roles.FirstOrDefault(r => r.Id == userRole.RoleId);

                CheckNotNull(user);
                CheckNotNull(role);

                user.Roles.Add(userRole);

            }
        }

        private void InitUserClaims(CacheDataReader userClaimsReader, IdentityDbContext context)
        {
            while (userClaimsReader.Read())
            {
                var userClaim = new IdentityUserClaim();
                userClaim.Id = (int)userClaimsReader["Id"];
                userClaim.ClaimType = (string)userClaimsReader["ClaimType"];
                userClaim.ClaimValue = (string)userClaimsReader["ClaimValue"];
                userClaim.UserId = (string)userClaimsReader["UserId"];

                var user = GetUser(context, userClaim.UserId);
                CheckNotNull(user);

                user.Claims.Add(userClaim);
            }
        }

        private void InitUserLogin(CacheDataReader userLoginReader, IdentityDbContext context)
        {
            while (userLoginReader.Read())
            {
                var userLogin = new IdentityUserLogin();
                userLogin.LoginProvider = (string)userLoginReader["LoginProvider"];
                userLogin.ProviderKey = (string)userLoginReader["ProviderKey"];
                userLogin.UserId = (string)userLoginReader["UserId"];

                var user = GetUser(context, userLogin.UserId);
            }
        }

        #endregion

        protected override void Seed(IdentityDbContext context)
        {
            base.Seed(context);

            using (var connection = BuildConnection(context))
            {
                using (CacheCommand usersCommand = new CacheCommand(UsersQuery, connection),
                       userRolesCommand = new CacheCommand(UserRolesQuery, connection),
                       rolesCommand = new CacheCommand(RolesQuery, connection),
                       userClaimsCommand = new CacheCommand(UserClaimsQuery, connection),
                       userLoginsCommand = new CacheCommand(UserLoginsQuery, connection))
                using (CacheDataReader usersReader = usersCommand.ExecuteReader(),
                       rolesReader = rolesCommand.ExecuteReader(),
                       userRolesReader = userRolesCommand.ExecuteReader(),
                       userClaimsReader = userClaimsCommand.ExecuteReader(),
                       userLoginReader = userLoginsCommand.ExecuteReader())
                {
                    InitUsers(usersReader, context);
                    InitRoles(rolesReader, context);
                    InitUserRoles(userRolesReader, context);
                    InitUserClaims(userClaimsReader, context);
                    InitUserLogin(userLoginReader, context);
                }
            }
        }
    }
}