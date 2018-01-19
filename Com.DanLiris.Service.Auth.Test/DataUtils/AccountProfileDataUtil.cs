using Com.DanLiris.Service.Auth.Lib.Models;

namespace Com.DanLiris.Service.Auth.Test.DataUtils
{
    public class AccountProfileDataUtil
    {
        public AccountProfile GetNewData()
        {
            AccountProfile TestData = new AccountProfile
            {
               Firstname = "Test Firstname",
               Lastname = "Test Lastname",
               Gender = "Male"
            };

            return TestData;
        }
    }
}
