using Com.DanLiris.Service.Auth.Lib.Models;

namespace Com.DanLiris.Service.Auth.Test.DataUtils
{
    public class PermissionDataUtil
    {
        public Permission GetTestData()
        {
            Permission TestData = new Permission
            {
                UnitId = 1,
                UnitCode = "Test Code",
                Unit = "Test Unit",
                Division = "Test Division",
                permission = 9
            };

            return TestData;
        }
    }
}
