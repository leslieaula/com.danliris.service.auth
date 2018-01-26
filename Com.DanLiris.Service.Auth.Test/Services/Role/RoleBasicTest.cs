using System;
using Xunit;
using Com.DanLiris.Service.Auth.Lib;
using Com.DanLiris.Service.Auth.Lib.Services;
using System.Collections.Generic;
using Com.DanLiris.Service.Auth.Test.Helpers;
using Models = Com.DanLiris.Service.Auth.Lib.Models;
using Com.DanLiris.Service.Auth.Test.DataUtils;

namespace Com.DanLiris.Service.Auth.Test.Service.Account
{
    [Collection("ServiceProviderFixture Collection")]
    public class RoleBasicTest : BasicServiceTest<AuthDbContext, RoleService, Models.Role, RoleDataUtil>
    {
        private static List<string> Keys = new List<string> { "Code" };
        private IServiceProvider serviceProvider { get; set; }

        public RoleBasicTest(ServiceProviderFixture fixture) : base(fixture, Keys)
        {
            serviceProvider = fixture.ServiceProvider;
        }
    }
}