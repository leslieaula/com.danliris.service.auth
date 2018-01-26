using Com.DanLiris.Service.Auth.Lib.Helpers;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Com.DanLiris.Service.Auth.Test.Helpers
{
    public abstract class BasicServiceTest<TDbContext, TService, TModel, TDataUtil>
        where TDbContext : DbContext
        where TService : BasicService<TDbContext, TModel>
        where TModel : StandardEntity, IValidatableObject, new()
        where TDataUtil : BasicDataUtil<TDbContext, TService, TModel>
    {
        private IServiceProvider serviceProvider { get; set; }
        private readonly List<string> Keys;

        public BasicServiceTest(ServiceProviderFixture fixture, List<string> keys)
        {
            serviceProvider = fixture.ServiceProvider;
            Keys = keys;
        }

        protected TDataUtil DataUtil
        {
            get { return (TDataUtil)this.serviceProvider.GetService(typeof(TDataUtil)); }
        }

        protected TService Service
        {
            get
            {
                TService service = (TService)this.serviceProvider.GetService(typeof(TService));
                service.Username = "Unit Test";

                return service;
            }
        }

        protected TDbContext DbContext
        {
            get { return (TDbContext)this.serviceProvider.GetService(typeof(TDbContext)); }
        }

        [Fact]
        public void Should_Success_Create_Data()
        {
            TModel Data = DataUtil.GetNewData(General.SERVICE_TEST_DATA);
            int AffectedRows = this.Service.CreateData(Data);

            Assert.True(AffectedRows > 0);
        }

        [Fact]
        public void Should_Success_Update_Data()
        {
            TModel Data = DataUtil.GetTestData();
            int AffectedRows = this.Service.UpdateData(Data.Id, Data);

            Assert.True(AffectedRows > 0);
        }

        [SkippableFact]
        public void Should_Error_Create_Data_With_Same_Keys()
        {
            Skip.If(Keys.Count == 0, "No Keys");

            try
            {
                TModel Data = DataUtil.GetTestData();
                Data.Id = 0;

                this.Service.CreateData(Data);
            }
            catch (ServiceValidationExeption ex)
            {
                foreach (string Key in Keys)
                {
                    ValidationResult result = ex.ValidationResults.FirstOrDefault(r => r.MemberNames.Contains(Key, StringComparer.CurrentCultureIgnoreCase));
                    Assert.NotNull(result);
                }
            }
        }

        [SkippableFact]
        public void Should_Error_Update_Data_With_Same_Keys()
        {
            Skip.If(Keys.Count == 0, "No Keys");

            try
            {
                TModel Data = DataUtil.GetTestData();
                TModel UpdateData = DataUtil.GetTestData();

                foreach (string Key in this.Keys)
                {
                    string Value = (string)Data.GetType().GetProperty(Key).GetValue(Data, null);
                    UpdateData.GetType().GetProperty(Key).SetValue(UpdateData, Value);
                }

                this.Service.UpdateData(UpdateData.Id, UpdateData);
            }
            catch (ServiceValidationExeption ex)
            {
                foreach (string Key in Keys)
                {
                    ValidationResult result = ex.ValidationResults.FirstOrDefault(r => r.MemberNames.Contains(Key, StringComparer.CurrentCultureIgnoreCase));
                    Assert.NotNull(result);
                }
            }
        }

        [Fact]
        public void Should_Success_Delete_Data()
        {
            TModel Data = DataUtil.GetTestData();
            int AffectedRows = this.Service.DeleteData(Data.Id);

            Assert.True(AffectedRows > 0);
        }

        [Fact]
        public void Should_Success_Get_Data()
        {
            TModel Data = DataUtil.GetTestData();
            var Response = this.Service.ReadData();

            Assert.NotEqual(Response.Item1.Count, 0);
        }

        [Fact]
        public void Should_Success_Get_Data_By_Id()
        {
            TModel Data = DataUtil.GetTestData();

            var Response = this.Service.ReadDataById(Data.Id);

            Assert.True(!Response.Equals(null));
        }
    }
}
