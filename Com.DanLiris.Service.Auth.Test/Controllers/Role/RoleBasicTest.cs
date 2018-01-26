using Com.DanLiris.Service.Auth.Lib.Services;
using Com.DanLiris.Service.Auth.Lib.ViewModels;
using Com.DanLiris.Service.Auth.Test.DataUtils;
using Xunit;
using System.Collections.Generic;
using Com.DanLiris.Service.Auth.Lib;
using Models = Com.DanLiris.Service.Auth.Lib.Models;
using Com.DanLiris.Service.Auth.Test.Controllers.Helpers;

namespace Com.DanLiris.Service.Auth.Test.Controllers.Role
{
    [Collection("TestServerFixture Collection")]
    public class RoleBasicTest : BasicControllerTest<AuthDbContext, RoleService, Models.Role, RoleViewModel, RoleDataUtil>
    {
        private static string URI = "v1/roles";
        private static List<string> CreateValidationAttributes = new List<string> { "Code", "Name", "Unit" };
        private static List<string> UpdateValidationAttributes = new List<string> { "Code", "Name", "Unit" };

        public RoleBasicTest(TestServerFixture fixture) : base(fixture, URI, CreateValidationAttributes, UpdateValidationAttributes)
        {
        }
    }
}
