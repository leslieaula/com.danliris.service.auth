using Com.DanLiris.Service.Auth.Lib;
using Com.DanLiris.Service.Auth.Lib.Models;
using Com.DanLiris.Service.Auth.Lib.Services;
using Com.DanLiris.Service.Auth.Lib.ViewModels;
using Com.DanLiris.Service.Auth.Test.Helpers;
using Com.DanLiris.Service.Auth.Test.Interfaces;
using System;
using System.Collections.Generic;

namespace Com.DanLiris.Service.Auth.Test.DataUtils
{
    public class AccountDataUtil : BasicDataUtil<AuthDbContext, AccountService, Account>, IEmptyData<AccountViewModel>
    {
        public AccountProfileDataUtil AccountProfileDataUtil { get; set; }
        public RoleDataUtil RoleDataUtil { get; set; }

        public AccountDataUtil(AuthDbContext dbContext, AccountService accountService, AccountProfileDataUtil accountProfileDataUtil, RoleDataUtil roleDataUtil) : base(dbContext, accountService)
        {
            this.AccountProfileDataUtil = accountProfileDataUtil;
            this.RoleDataUtil = roleDataUtil;
        }

        public override Account GetNewData(string Type)
        {
            string guid = Guid.NewGuid().ToString();
            Role role = RoleDataUtil.GetTestData();

            AccountRole accountRole = null;

            switch (Type)
            {
                case General.CONTROLLER_TEST_DATA: accountRole = new AccountRole { RoleId = role.Id, Role = role }; break;
                case General.SERVICE_TEST_DATA: accountRole = new AccountRole { RoleId = role.Id }; break;
            }

            Account TestData = new Account
            {
                Username = guid,
                Password = "Password",
                IsLocked = false,
                AccountProfile = AccountProfileDataUtil.GetTestData(),
                AccountRoles = new List<AccountRole> { accountRole }
            };

            return TestData;
        }

        public override Account GetTestData()
        {
            Account Data = GetNewData(General.SERVICE_TEST_DATA);

            this.Service.CreateData(Data);
            return Data;
        }

        public AccountViewModel GetEmptyData()
        {
            AccountViewModel Data = new AccountViewModel();

            Data.username = string.Empty;
            Data.password = string.Empty;
            Data.roles = new List<RoleViewModel> { new RoleViewModel() };

            return Data;
        }
    }
}