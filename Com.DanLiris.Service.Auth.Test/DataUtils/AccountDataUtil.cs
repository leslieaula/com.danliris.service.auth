using Com.DanLiris.Service.Auth.Lib;
using Com.DanLiris.Service.Auth.Lib.Models;
using Com.DanLiris.Service.Auth.Lib.Services;
using Com.DanLiris.Service.Auth.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.DanLiris.Service.Auth.Test.DataUtils
{
    public class AccountDataUtil : BasicDataUtil<AuthDbContext, AccountService, Account>
    {
        public AccountProfileDataUtil AccountProfileDataUtil { get; set; }
        public RoleDataUtil RoleDataUtil { get; set; }

        public AccountDataUtil(AuthDbContext dbContext, AccountService accountService, AccountProfileDataUtil accountProfileDataUtil, RoleDataUtil roleDataUtil) : base(dbContext, accountService)
        {
            this.AccountProfileDataUtil = accountProfileDataUtil;
            this.RoleDataUtil = roleDataUtil;
        }

        public override Account GetNewData()
        {
            string guid = Guid.NewGuid().ToString();
            Role role = RoleDataUtil.GetTestData();

            AccountRole accountRole = new AccountRole { RoleId = role.Id };

            Account TestData = new Account
            {
                Username = guid,
                Password = "Password",
                IsLocked = false,
                AccountProfile = AccountProfileDataUtil.GetNewData(),
                AccountRoles = new List<AccountRole> { accountRole }
            };

            return TestData;
        }

        public override Account GetTestData()
        {
            Account Data = GetNewData();
            Account TestData = this.Service.DbSet.FirstOrDefault(account => account.Username.Equals(Data.Username));

            if (TestData != null)
                return TestData;
            else
            {
                this.Service.CreateData(Data);
                return Data;
            }
        }
    }
}