using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib.Service;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Com.DanLiris.Service.Auth.Lib.Interfaces;

namespace Com.DanLiris.Service.Auth.Lib.Helpers
{
    public abstract class BasicService<TDbContext, TModel> : StandardEntityService<TDbContext, TModel>
        where TDbContext : DbContext
        where TModel : StandardEntity, IValidatableObject
    {
        public string Username { get; set; }

        public BasicService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public virtual int CreateData(TModel Model)
        {
            return this.Create(Model);
        }

        public abstract Tuple<List<TModel>, int, Dictionary<string, string>, List<string>> ReadData(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null);

        public virtual TModel ReadDataById(int Id)
        {
            return this.Get(Id);
        }

        public virtual int UpdateData(int Id, TModel Model)
        {
            return this.Update(Id, Model);
        }

        public virtual int DeleteData(int Id)
        {
            return this.Delete(Id);
        }
    }
}