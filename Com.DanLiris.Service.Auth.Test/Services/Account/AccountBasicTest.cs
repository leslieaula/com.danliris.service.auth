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
    public class AccountBasicTest : BasicServiceTest<AuthDbContext, AccountService, Models.Account, AccountDataUtil>
    {
        private static List<string> Keys = new List<string> { "Username" };

        public AccountBasicTest(ServiceProviderFixture fixture) : base(fixture, Keys)
        {
        }
    }
}