﻿using System;
using System.Collections.Generic;
using System.Linq;
using Kodkod.Core.AppConsts;
using Kodkod.Core.Entities;

namespace Kodkod.EntityFramework
{
    public class SeedData
    {
        #region private fiels
        private static readonly ApplicationUser AdminUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "admin",
            Email = "admin@mail.com",
            EmailConfirmed = true,
            NormalizedEmail = "ADMIN@MAIL.COM",
            NormalizedUserName = "ADMIN",
            PasswordHash = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
        };

        private static readonly ApplicationUser TestUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "testuser",
            Email = "testuser@mail.com",
            EmailConfirmed = true,
            NormalizedEmail = "TESTUSER@MAIL.COM",
            NormalizedUserName = "TESTUSER",
            PasswordHash = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
        };

        private static readonly ApplicationRole AdminRole = new ApplicationRole
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            NormalizedName = "ADMIN"
        };
        #endregion

        public static List<ApplicationUser> BuildApplicationUsers() => new List<ApplicationUser>
        {
            AdminUser,
            TestUser
        };

        public static List<ApplicationRole> BuildApplicationRoles() => new List<ApplicationRole>
        {
            AdminRole
        };

        public static List<ApplicationUserRole> BuildApplicationUserRoles() => new List<ApplicationUserRole>
        {
            new ApplicationUserRole
            {
                RoleId = AdminRole.Id,
                UserId = AdminUser.Id
            }
        };

        public static List<Permission> BuildPermissions() => KodkodPermissions.AllPermissions();

        public static List<RolePermission> BuildRolePermissions() => KodkodPermissions.AllPermissions().Select(p =>
            new RolePermission
            {
                PermissionId = p.Id,
                RoleId = AdminRole.Id
            }).ToList();
    }
}
