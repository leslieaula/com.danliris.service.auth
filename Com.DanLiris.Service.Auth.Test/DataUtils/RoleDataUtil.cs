using Com.DanLiris.Service.Auth.Lib;
using Com.DanLiris.Service.Auth.Lib.Models;
using Com.DanLiris.Service.Auth.Lib.Services;
using Com.DanLiris.Service.Auth.Test.DataUtils;
using Com.DanLiris.Service.Auth.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.DanLiris.Service.Auth.Test.DataUtils
{
    public class RoleDataUtil : BasicDataUtil<AuthDbContext, RoleService, Role>
    {
        public PermissionDataUtil PermissionDataUtil { get; set; }

        public RoleDataUtil(AuthDbContext dbContext, RoleService roleService, PermissionDataUtil permissionDataUtil) : base(dbContext, roleService)
        {
            this.PermissionDataUtil = permissionDataUtil;
        }

        public override Role GetNewData()
        {
            string guid = Guid.NewGuid().ToString();
  
            Role TestData = new Role
            {
                Name = "Test Name",
                Code = guid,
                Description = "Test Description",
                Permissions = new List<Permission> { PermissionDataUtil.GetNewData() }
            };

            return TestData;
        }

        public override Role GetTestData()
        {
            Role Data = GetNewData();
            Role TestData = this.Service.DbSet.FirstOrDefault(role => role.Code.Equals(Data.Code));

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