using Com.DanLiris.Service.Auth.Lib.Helpers;
using Com.DanLiris.Service.Auth.Lib.Models;
using Com.DanLiris.Service.Auth.Lib.ViewModels;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Com.DanLiris.Service.Auth.Lib.Interfaces;

namespace Com.DanLiris.Service.Auth.Lib.Services
{
    public class RoleService : BasicService<AuthDbContext, Role>, IMap<Role, RoleViewModel>
    {
        private readonly MappingEntity<Role, RoleViewModel> mappingEntity;
        public RoleService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.mappingEntity = new MappingEntity<Role, RoleViewModel>();
        }

        public override Tuple<List<Role>, int, Dictionary<string, string>, List<string>> ReadData(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null)
        {
            IQueryable<Role> Query = this.DbContext.Roles;
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Name"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "_id", "code", "name", "description", "permissions"
            };

            Query = Query
                .Select(a => new Role
                {
                    Id = a.Id,
                    Code = a.Code,
                    Name = a.Name,
                    Description = a.Description,
                    Permissions = a.Permissions.Where(p => p.RoleId.Equals(a.Id)).ToList()
                });

            /* Order */
            if (OrderDictionary.Count.Equals(0))
            {
                OrderDictionary.Add("_updatedDate", General.DESCENDING);

                Query = Query.OrderByDescending(b => b._LastModifiedUtc); /* Default Order */
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];
                string TransformKey = General.TransformOrderBy(Key);

                BindingFlags IgnoreCase = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

                Query = OrderType.Equals(General.ASCENDING) ?
                    Query.OrderBy(b => b.GetType().GetProperty(TransformKey, IgnoreCase).GetValue(b)) :
                    Query.OrderByDescending(b => b.GetType().GetProperty(TransformKey, IgnoreCase).GetValue(b));
            }

            /* Pagination */
            Pageable<Role> pageable = new Pageable<Role>(Query, Page - 1, Size);
            List<Role> Data = pageable.Data.ToList<Role>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public Role MapToModel(RoleViewModel roleVM)
        {
            Role role = new Role();
            role.Permissions = new List<Permission>();

            this.mappingEntity.mappingToNewEntity(role, roleVM);

            role.Code = roleVM.code;
            role.Name = roleVM.name;
            role.Description = roleVM.description;

            foreach (PermissionViewModel permissionVM in roleVM.permissions)
            {
                Permission permission = new Permission();

                permission.Id = permissionVM.id;
                permission._CreatedBy = permissionVM._createdBy;
                permission._CreatedAgent = permissionVM._createAgent;
                permission._CreatedUtc = permissionVM._createdDate;
                permission.RoleId = permissionVM.roleId;
                permission.UnitId = permissionVM.unit._id;
                permission.Unit = permissionVM.unit.name;
                permission.UnitCode = permissionVM.unit.code;
                permission.permission = permissionVM.permission;
                permission.Division = permissionVM.unit.division != null ? permissionVM.unit.division.name : null;

                role.Permissions.Add(permission);
            }

            return role;
        }

        public RoleViewModel MapToViewModel(Role role)
        {
            RoleViewModel roleVM = new RoleViewModel();
            roleVM.permissions = new List<PermissionViewModel>();

            this.mappingEntity.MappingToOldEntity(role, roleVM);

            roleVM.code = role.Code;
            roleVM.name = role.Name;
            roleVM.description = role.Description;

            if (role.Permissions != null)
            {
                foreach (Permission permission in role.Permissions)
                {
                    PermissionViewModel permissionVM = new PermissionViewModel();
                    permissionVM.unit = new UnitViewModel();
                    permissionVM.unit.division = new DivisionViewModel();

                    permissionVM.id = permission.Id;
                    permissionVM._createdBy = permission._CreatedBy;
                    permissionVM._createAgent = permission._CreatedAgent;
                    permissionVM._createdDate = permission._CreatedUtc;
                    permissionVM.roleId = permission.RoleId;
                    permissionVM.unit._id = permission.UnitId;
                    permissionVM.unit.name = permission.Unit;
                    permissionVM.unit.code = permission.UnitCode;
                    permissionVM.permission = permission.permission;
                    permissionVM.unit.division.name = permission.Division;

                    roleVM.permissions.Add(permissionVM);
                }
            }

            return roleVM;
        }

        public override Role ReadDataById(int Id)
        {
            return this.DbSet.Where(d => d.Id.Equals(Id)).Include(i => i.Permissions).FirstOrDefault();
        }

        public override int CreateData(Role role)
        {
            PermissionService permissionService = ServiceProvider.GetService<PermissionService>();
            permissionService.Username = this.Username;

            foreach (Permission permission in role.Permissions)
            {
                permissionService.OnCreating(permission);
            }

            int Count = this.Create(role);

            return Count;
        }

        public override int UpdateData(int Id, Role role)
        {
            int Count = 0;

            using (var Transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    PermissionService permissionService = ServiceProvider.GetService<PermissionService>();
                    permissionService.Username = this.Username;

                    HashSet<int> Permissions = new HashSet<int>(this.DbContext.Permissions.Where(p => p.RoleId.Equals(Id)).Select(p => p.Id));

                    foreach (int permission in Permissions)
                    {
                        Permission p = role.Permissions.FirstOrDefault(prop => prop.Id.Equals(permission));

                        if (p != null)
                        {
                            permissionService.UpdateData(p.Id, p);
                        }
                        else
                        {
                            permissionService.DeleteData(permission);
                        }
                    }

                    foreach (Permission permission in role.Permissions)
                    {
                        if (permission.Id.Equals(0))
                        {
                            permission.RoleId = Id;
                            permissionService.CreateData(permission);
                        }
                    }

                    Count = this.Update(Id, role);
                    Transaction.Commit();
                }
                catch (DbUpdateConcurrencyException)
                {
                    Transaction.Rollback();
                    throw;
                }
            }

            return Count;
        }

        public override int DeleteData(int Id)
        {
            using (var Transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    this.Delete(Id);

                    PermissionService permissionService = ServiceProvider.GetService<PermissionService>();
                    permissionService.Username = this.Username;

                    HashSet<int> Permissions = new HashSet<int>(this.DbContext.Permissions.Where(p => p.RoleId.Equals(Id)).Select(p => p.Id));

                    foreach (int permission in Permissions)
                    {
                        permissionService.DeleteData(permission);
                    }

                    Transaction.Commit();
                }
                catch (DbUpdateConcurrencyException)
                {
                    Transaction.Rollback();
                    throw;
                }
            }

            return Id;
        }

        public override void OnCreating(Role model)
        {
            base.OnCreating(model);
            model._CreatedAgent = "Service";
            model._CreatedBy = this.Username;
        }

        public override void OnUpdating(int id, Role model)
        {
            base.OnUpdating(id, model);
            model._LastModifiedAgent = "Service";
            model._LastModifiedBy = this.Username;
        }

        public override void OnDeleting(Role model)
        {
            base.OnDeleting(model);
            model._LastModifiedAgent = "Service";
            model._LastModifiedBy = this.Username;
            model._DeletedAgent = "Service";
            model._DeletedBy = this.Username;
        }
    }
}
