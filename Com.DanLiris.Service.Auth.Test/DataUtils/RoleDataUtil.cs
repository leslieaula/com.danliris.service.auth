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
    public class RoleDataUtil : BasicDataUtil<AuthDbContext, RoleService, Role>, IEmptyData<RoleViewModel>
    {
        public PermissionDataUtil PermissionDataUtil { get; set; }

        public RoleDataUtil(AuthDbContext dbContext, RoleService roleService, PermissionDataUtil permissionDataUtil) : base(dbContext, roleService)
        {
            this.PermissionDataUtil = permissionDataUtil;
        }

        public override Role GetNewData(string Type)
        {
            string guid = Guid.NewGuid().ToString();
  
            Role TestData = new Role
            {
                Name = "Test Name",
                Code = guid,
                Description = "Test Description",
                Permissions = new List<Permission> { PermissionDataUtil.GetTestData() }
            };

            return TestData;
        }

        public override Role GetTestData()
        {
            Role Data = GetNewData(General.SERVICE_TEST_DATA);

            this.Service.CreateData(Data);
            return Data;
        }

        public RoleViewModel GetEmptyData()
        {
            RoleViewModel Data = new RoleViewModel();
            Data.code = string.Empty;
            Data.name = string.Empty;
            Data.description = string.Empty;
            Data.permissions = new List<PermissionViewModel> { new PermissionViewModel { unit = new UnitViewModel() } };

            return Data;
        }
    }
}