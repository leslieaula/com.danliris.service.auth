using Com.DanLiris.Service.Auth.Lib.Services;
using Com.DanLiris.Service.Auth.Lib.ViewModels;
using Com.DanLiris.Service.Auth.Test.DataUtils;
using Xunit;
using System.Collections.Generic;
using Com.DanLiris.Service.Auth.Lib;
using Models = Com.DanLiris.Service.Auth.Lib.Models;
using Com.DanLiris.Service.Auth.Test.Controllers.Helpers;

namespace Com.DanLiris.Service.Auth.Test.Controllers.Account
{
    [Collection("TestServerFixture Collection")]
    public class AccountBasicTest : BasicControllerTest<AuthDbContext, AccountService, Models.Account, AccountViewModel, AccountDataUtil>
    {
        private static string URI = "v1/accounts";
        private static List<string> CreateValidationAttributes = new List<string> { "Username", "Password", "Profile", "Roles" };
        private static List<string> UpdateValidationAttributes = new List<string> { "Username", "Profile", "Roles" };

        public AccountBasicTest(TestServerFixture fixture) : base(fixture, URI, CreateValidationAttributes, UpdateValidationAttributes)
        {
        }
    }
}
