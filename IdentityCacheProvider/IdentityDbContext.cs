// Copyright (c) KHNURE, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;

namespace Intersystems.AspNet.Identity.Cache
{
    /// <summary>
    /// Default IdentityDbContext that uses the default entity types for ASP.NET Identity Users, Roles, Claims, Logins. 
    /// Use this overload to add your own entity types.
    /// </summary>
    public class IdentityDbContext :
        IdentityDbContext<IdentityUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        /// <summary>
        ///     Default constructor which uses the DefaultConnection
        /// </summary>
        public IdentityDbContext()
            : this("DefaultConnection")
        {
        }

        /// <summary>
        ///     Constructor which takes the connection string to use
        /// </summary>
        /// <param name="nameOrConnectionString"></param>
        public IdentityDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        /// <summary>
        ///     Constructs a new context instance using the existing connection to connect to a database, and initializes it from
        ///     the given model.  The connection will not be disposed when the context is disposed if contextOwnsConnection is
        ///     false.
        /// </summary>
        /// <param name="existingConnection">An existing connection to use for the new context.</param>
        /// <param name="model">The model that will back this context.</param>
        /// <param name="contextOwnsConnection">
        ///     Constructs a new context instance using the existing connection to connect to a
        ///     database, and initializes it from the given model.  The connection will not be disposed when the context is
        ///     disposed if contextOwnsConnection is false.
        /// </param>
        public IdentityDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }

        /// <summary>
        ///     Constructs a new context instance using conventions to create the name of
        ///     the database to which a connection will be made, and initializes it from
        ///     the given model.  The by-convention name is the full name (namespace + class
        ///     name) of the derived context class.
        /// </summary>
        /// <param name="model">The model that will back this context.</param>
        public IdentityDbContext(DbCompiledModel model)
            : base(model)
        {
        }

        /// <summary>
        ///     Constructs a new context instance using the existing connection to connect
        ///     to a database.  The connection will not be disposed when the context is disposed
        ///     if contextOwnsConnection is false.
        /// </summary>
        /// <param name="existingConnection">An existing connection to use for the new context.</param>
        /// <param name="contextOwnsConnection">If set to true the connection is disposed when the context is disposed, otherwise
        ///     the caller must dispose the connection.
        /// </param>
        public IdentityDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        /// <summary>
        ///     Constructs a new context instance using the given string as the name or connection
        ///     string for the database to which a connection will be made, and initializes
        ///     it from the given model.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <param name="model">The model that will back this context.</param>
        public IdentityDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
        }
    }

    /// <summary>
    /// Generic IdentityDbContext base that can be customized with entity types that extend from the base IdentityUserXXX types.
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TRole"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TUserLogin"></typeparam>
    /// <typeparam name="TUserRole"></typeparam>
    /// <typeparam name="TUserClaim"></typeparam>
    public class IdentityDbContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> : DbContext
        where TUser : IdentityUser<TKey, TUserLogin, TUserRole, TUserClaim>
        where TRole : IdentityRole<TKey, TUserRole>
        where TUserLogin : IdentityUserLogin<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
    {
        /// <summary>
        ///     Default constructor which uses the "DefaultConnection" connectionString
        /// </summary>
        public IdentityDbContext()
            : this("DefaultConnection")
        {
            Database.SetInitializer<IdentityDbContext>(new IdentityDbInitializer());
        }

        /// <summary>
        ///     Constructor which takes the connection string to use
        /// </summary>
        /// <param name="nameOrConnectionString"></param>
        public IdentityDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Database.SetInitializer<IdentityDbContext>(new IdentityDbInitializer());
        }

        /// <summary>
        ///     Constructs a new context instance using the existing connection to connect to a database, and initializes it from
        ///     the given model.  The connection will not be disposed when the context is disposed if contextOwnsConnection is
        ///     false.
        /// </summary>
        /// <param name="existingConnection">An existing connection to use for the new context.</param>
        /// <param name="model">The model that will back this context.</param>
        /// <param name="contextOwnsConnection">
        ///     Constructs a new context instance using the existing connection to connect to a
        ///     database, and initializes it from the given model.  The connection will not be disposed when the context is
        ///     disposed if contextOwnsConnection is false.
        /// </param>
        public IdentityDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
            Database.SetInitializer<IdentityDbContext>(new IdentityDbInitializer());
        }

        /// <summary>
        ///     Constructs a new context instance using conventions to create the name of
        ///     the database to which a connection will be made, and initializes it from
        ///     the given model.  The by-convention name is the full name (namespace + class
        ///     name) of the derived context class.
        /// </summary>
        /// <param name="model">The model that will back this context.</param>
        public IdentityDbContext(DbCompiledModel model)
            : base(model)
        {
            Database.SetInitializer<IdentityDbContext>(new IdentityDbInitializer());
        }

        /// <summary>
        ///     Constructs a new context instance using the existing connection to connect
        ///     to a database.  The connection will not be disposed when the context is disposed
        ///     if contextOwnsConnection is false.
        /// </summary>
        /// <param name="existingConnection">An existing connection to use for the new context.</param>
        /// <param name="contextOwnsConnection">If set to true the connection is disposed when the context is disposed, otherwise
        ///     the caller must dispose the connection.
        /// </param>
        public IdentityDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            Database.SetInitializer<IdentityDbContext>(new IdentityDbInitializer());
        }

        /// <summary>
        ///     Constructs a new context instance using the given string as the name or connection
        ///     string for the database to which a connection will be made, and initializes
        ///     it from the given model.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <param name="model">The model that will back this context.</param>
        public IdentityDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            Database.SetInitializer<IdentityDbContext>(new IdentityDbInitializer());
        }

        /// <summary>
        ///     IDbSet of Users
        /// </summary>
        public virtual IDbSet<TUser> Users { get; set; }

        /// <summary>
        ///     IDbSet of Roles
        /// </summary>
        public virtual IDbSet<TRole> Roles { get; set; }

        /// <summary>
        ///     If true validates that emails are unique
        /// </summary>
        public bool RequireUniqueEmail { get; set; }

        /// <summary>
        ///     Maps table names, and sets up relationships between the various user entities
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }

            // Needed to ensure subclasses share the same table
            var user = modelBuilder.Entity<TUser>()
                .ToTable("AspNetUsers");
            user.HasMany(u => u.Roles).WithRequired().HasForeignKey(ur => ur.UserId);
            user.HasMany(u => u.Claims).WithRequired().HasForeignKey(uc => uc.UserId);
            user.HasMany(u => u.Logins).WithRequired().HasForeignKey(ul => ul.UserId);
            user.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex") { IsUnique = true }));

            // CONSIDER: u.Email is Required if set on options?
            user.Property(u => u.Email).HasMaxLength(256);

            modelBuilder.Entity<TUserRole>()
                .HasKey(r => new { r.UserId, r.RoleId })
                .ToTable("AspNetUserRoles");

            modelBuilder.Entity<TUserLogin>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey, l.UserId })
                .ToTable("AspNetUserLogins");

            modelBuilder.Entity<TUserClaim>()
                .ToTable("AspNetUserClaims");

            var role = modelBuilder.Entity<TRole>()
                .ToTable("AspNetRoles");
            role.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("RoleNameIndex") { IsUnique = true }));
            role.HasMany(r => r.Users).WithRequired().HasForeignKey(ur => ur.RoleId);
        }

        /// <summary>
        ///     Validates that UserNames are unique and case insenstive
        /// </summary>
        /// <param name="entityEntry"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry,
            IDictionary<object, object> items)
        {
            if (entityEntry != null && entityEntry.State == EntityState.Added)
            {
                var errors = new List<DbValidationError>();
                var user = entityEntry.Entity as TUser;
                //check for uniqueness of user name and email
                if (user != null)
                {
                    if (Users.Any(u => String.Equals(u.UserName, user.UserName)))
                    {
                        errors.Add(new DbValidationError("User",
                            String.Format(CultureInfo.CurrentCulture, IdentityResources.DuplicateUserName, user.UserName)));
                    }
                    if (RequireUniqueEmail && Users.Any(u => String.Equals(u.Email, user.Email)))
                    {
                        errors.Add(new DbValidationError("User",
                            String.Format(CultureInfo.CurrentCulture, IdentityResources.DuplicateEmail, user.Email)));
                    }
                }
                else
                {
                    var role = entityEntry.Entity as TRole;
                    //check for uniqueness of role name
                    if (role != null && Roles.Any(r => String.Equals(r.Name, role.Name)))
                    {
                        errors.Add(new DbValidationError("Role",
                            String.Format(CultureInfo.CurrentCulture, IdentityResources.RoleAlreadyExists, role.Name)));
                    }
                }
                if (errors.Any())
                {
                    return new DbEntityValidationResult(entityEntry, errors);
                }
            }
            return base.ValidateEntity(entityEntry, items);
        }
    }
}