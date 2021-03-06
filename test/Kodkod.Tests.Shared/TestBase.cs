﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Kodkod.Core.Permissions;
using Kodkod.Core.Roles;
using Kodkod.Core.Users;
using Kodkod.EntityFramework;
using Kodkod.Utilities.Collections.Dictionary.Extensions;
using Kodkod.Web.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kodkod.Tests.Shared
{
    public class TestBase
    {
        private static readonly object ThisLock = new object();
        private static Dictionary<string, string> _testUserFormData;

        protected readonly KodkodDbContext KodkodInMemoryContext;
        protected readonly ClaimsPrincipal ContextUser;
        protected readonly HttpClient Client;
        protected async Task<HttpResponseMessage> LoginAsTestUserAsync()
        {
            return await Client.PostAsync("/api/account/login",
                _testUserFormData.ToStringContent(Encoding.UTF8, "application/json"));
        }

        public TestBase()
        {
            _testUserFormData = new Dictionary<string, string>
            {
                {"email", "testuser@mail.com"},
                {"username", "testuser"},
                {"password", "123qwe"}
            };

            KodkodInMemoryContext = GetInitializedDbContext();
            ContextUser = GetContextUser();

            //if this is true, Automapper is throwing exception
            //ServiceCollectionExtensions.UseStaticRegistration = false;

            lock (ThisLock)
            {
                Mapper.Reset();
                Client = GetTestServer();
            }
        }

        public static readonly User AdminUser = new User
        {
            Id = new Guid("C41A7761-6645-4E2C-B99D-F9E767B9AC77"),
            UserName = "admin"
        };

        public static readonly User TestUser = new User
        {
            Id = new Guid("065E903E-6F7B-42B8-B807-0C4197F9D1BC"),
            UserName = "testuser"
        };

        public static readonly Role AdminRole = new Role
        {
            Id = new Guid("F22BCE18-06EC-474A-B9AF-A9DE2A7B8263"),
            Name = "Admin"
        };

        public static readonly Role MemberRole = new Role
        {
            Id = new Guid("11D14A89-3A93-4D39-A94F-82B823F0D4CE"),
            Name = "Member"
        };

        public static readonly Permission ApiUserPermission = new Permission
        {
            Id = new Guid("41F04B93-8C0E-4AC2-B6BA-63C052A2F02A"),
            Name = "ApiUser"
        };

        public static readonly UserRole AdminUserRole = new UserRole
        {
            RoleId = AdminRole.Id,
            UserId = AdminUser.Id
        };

        public static readonly UserRole TestUserRole = new UserRole
        {
            RoleId = MemberRole.Id,
            UserId = TestUser.Id
        };

        public static readonly RolePermission AdminRolePermission = new RolePermission
        {
            PermissionId = ApiUserPermission.Id,
            RoleId = AdminRole.Id
        };

        public static readonly RolePermission MemberRolePermission = new RolePermission
        {
            PermissionId = ApiUserPermission.Id,
            RoleId = MemberRole.Id
        };

        public static readonly List<User> AllTestUsers = new List<User>
        {
            new User {UserName = "A"},
            new User {UserName = "B"},
            new User {UserName = "C"},
            new User {UserName = "D"},
            AdminUser,
            TestUser
        };

        public static KodkodDbContext GetEmptyDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<KodkodDbContext>();
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            optionsBuilder.UseLazyLoadingProxies();

            var inMemoryContext = new KodkodDbContext(optionsBuilder.Options);

            return inMemoryContext;
        }

        public static KodkodDbContext GetInitializedDbContext()
        {
            var inMemoryContext = GetEmptyDbContext();

            inMemoryContext.AddRange(ApiUserPermission);
            inMemoryContext.AddRange(AdminRole, MemberRole);
            inMemoryContext.AddRange(AllTestUsers);
            inMemoryContext.AddRange(AdminUserRole, TestUserRole);
            inMemoryContext.AddRange(AdminRolePermission, MemberRolePermission);

            inMemoryContext.SaveChanges();

            return inMemoryContext;
        }

        public static ClaimsPrincipal GetContextUser()
        {
            return new ClaimsPrincipal(
                  new ClaimsIdentity(
                      new List<Claim>
                      {
                        new Claim(ClaimTypes.Name, TestUser.UserName)
                      },
                      "TestAuthenticationType"
                  )
              );
        }

        private static HttpClient GetTestServer()
        {
            var server = new TestServer(
                new WebHostBuilder()
                    .UseStartup<Startup>()
                    .ConfigureAppConfiguration(config =>
                    {
                        config.SetBasePath(Path.GetFullPath(@"../../.."));
                        //todo:remove all appsettins.json from test projects and use from shared test project
                        config.AddJsonFile("appsettings.json", false);
                    })
            );

            return server.CreateClient();
        }
    }
}
