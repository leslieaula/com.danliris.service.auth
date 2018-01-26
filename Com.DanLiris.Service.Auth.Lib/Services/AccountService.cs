using Com.DanLiris.Service.Auth.Lib.Authentication;
using Com.DanLiris.Service.Auth.Lib.Models;
using Com.DanLiris.Service.Auth.Lib.ViewModels;
using Com.DanLiris.Service.Auth.Lib.Helpers;
using Com.Moonlay.NetCore.Lib;
using IdentityModel.Client;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading.Tasks;
using Com.DanLiris.Service.Auth.Lib.Interfaces;

namespace Com.DanLiris.Service.Auth.Lib.Services
{
    public class AccountService : BasicService<AuthDbContext, Account>, IMap<Account, AccountViewModel>
    {
        private readonly MappingEntity<Account, AccountViewModel> mappingEntitiy;
        public AccountService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.mappingEntitiy = new MappingEntity<Account, AccountViewModel>();
        }

        public async Task<string> Authenticate(string Username, string Password)
        {
            var disco = await DiscoveryClient.GetAsync(Config.Authority);
            if (disco.IsError)
            {
                throw new Exception(disco.Error);
            }

            var tokenClient = new TokenClient(disco.TokenEndpoint, Config.ClientId, Config.Secret);
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(Username, Password, "com.danliris.service");

            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.ErrorDescription);
            }

            Dictionary<string, object> response = JsonConvert.DeserializeObject<Dictionary<string, object>>(tokenResponse.Json.ToString());
            string token = response["access_token"].ToString();
            return token;
        }

        public override Tuple<List<Account>, int, Dictionary<string, string>, List<string>> ReadData(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null)
        {
            IQueryable<Account> Query = this.DbContext.Accounts;
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Username", "AccountProfile.FirstName", "AccountProfile.LastName"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "_id", "username", "profile"
            };

            Query = Query
                .Select(a => new Account
                {
                    Id = a.Id,
                    Username = a.Username,
                    AccountProfile = a.AccountProfile
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
            Pageable<Account> pageable = new Pageable<Account>(Query, Page - 1, Size);
            List<Account> Data = pageable.Data.ToList<Account>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public Account MapToModel(AccountViewModel accountVM)
        {
            Account account = new Account();
            account.AccountProfile = new AccountProfile();
            account.AccountRoles = new List<AccountRole>();

            this.mappingEntitiy.mappingToNewEntity(account, accountVM);

            account.Username = accountVM.username;
            account.Password = accountVM.password;
            account.IsLocked = accountVM.isLocked;
            account.AccountProfile.Id = accountVM.profile.id;
            account.AccountProfile.Firstname = accountVM.profile.firstname;
            account.AccountProfile.Lastname = accountVM.profile.lastname;
            account.AccountProfile.Gender = accountVM.profile.gender;
            account.AccountProfile.AccountId = accountVM._id;

            foreach (RoleViewModel roleVM in accountVM.roles)
            {
                AccountRole accountRole = new AccountRole();
                accountRole.RoleId = roleVM._id;

                account.AccountRoles.Add(accountRole);
            }

            return account;
        }

        public AccountViewModel MapToViewModel(Account account)
        {
            AccountViewModel accountVM = new AccountViewModel();
            accountVM.profile = new AccountProfileViewModel();
            accountVM.roles = new List<RoleViewModel>();

            this.mappingEntitiy.MappingToOldEntity(account, accountVM);

            accountVM.username = account.Username;
            accountVM.password = account.Password;
            accountVM.isLocked = account.IsLocked;
            accountVM.profile.id = account.AccountProfile.Id;
            accountVM.profile.firstname = account.AccountProfile.Firstname;
            accountVM.profile.lastname = account.AccountProfile.Lastname;
            accountVM.profile.gender = account.AccountProfile.Gender;

            if (account.AccountRoles != null)
            {
                foreach (AccountRole accountRole in account.AccountRoles)
                {
                    RoleViewModel roleViewModel = new RoleViewModel();
                    roleViewModel._id = accountRole.RoleId;
                    roleViewModel.name = accountRole.Role.Name;
                    roleViewModel.code = accountRole.Role.Code;

                    accountVM.roles.Add(roleViewModel);
                }
            }
            return accountVM;
        }

        public override Account ReadDataById(int Id)
        {
            return this.DbSet
                .Where(d => d.Id.Equals(Id))
                .Include(i => i.AccountProfile)
                .Include(i => i.AccountRoles)
                    .ThenInclude(i => i.Role)
                .FirstOrDefault();
        }

        public override int CreateData(Account account)
        {
            AccountRoleService accountRoleService = ServiceProvider.GetService<AccountRoleService>();
            accountRoleService.Username = this.Username;

            foreach (AccountRole accountRole in account.AccountRoles)
            {
                accountRoleService.OnCreating(accountRole);
            }

            int Count = this.Create(account);

            return Count;
        }

        public override int UpdateData(int Id, Account account)
        {
            int Count = 0;

            using (var Transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    this.DbContext.Entry(account).State = EntityState.Modified;

                    if (string.IsNullOrWhiteSpace(account.Password))
                    {
                        this.DbContext.Entry(account).Property(p => p.Password).IsModified = false;
                    }

                    this.OnUpdating(Id, account);
                    this.DbContext.AccountProfiles.Update(account.AccountProfile);

                    AccountRoleService accountRoleService = ServiceProvider.GetService<AccountRoleService>();
                    accountRoleService.Username = this.Username;

                    HashSet<int> AccountRoles = new HashSet<int>(this.DbContext.AccountRoles.Where(p => p.AccountId.Equals(Id)).Select(p => p.Id));

                    foreach (int accountRole in AccountRoles)
                    {
                        AccountRole a = account.AccountRoles.FirstOrDefault(prop => prop.Id.Equals(accountRole));

                        if(a == null)
                        {
                            accountRoleService.DeleteData(accountRole);
                        }
                    }

                    foreach (AccountRole accountRole in account.AccountRoles)
                    {
                        if (accountRole.Id.Equals(0))
                        {
                            accountRoleService.CreateData(accountRole);
                        }
                    }

                    Count = this.DbContext.SaveChanges();
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
            int Count = 0;

            using (var Transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    Count = this.Delete(Id);

                    AccountRoleService accountRoleService = ServiceProvider.GetService<AccountRoleService>();
                    accountRoleService.Username = this.Username;

                    HashSet<int> AccountRoles = new HashSet<int>(this.DbContext.AccountRoles.Where(p => p.AccountId.Equals(Id)).Select(p => p.Id));

                    foreach (int accountRole in AccountRoles)
                    {
                        accountRoleService.DeleteData(accountRole);
                    }

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

        public override void OnCreating(Account model)
        {
            base.OnCreating(model);
            model._CreatedAgent = "Service";
            model._CreatedBy = this.Username;
        }

        public override void OnUpdating(int id, Account model)
        {
            base.OnUpdating(id, model);
            model._LastModifiedAgent = "Service";
            model._LastModifiedBy = this.Username;
        }

        public override void OnDeleting(Account model)
        {
            base.OnDeleting(model);
            model._LastModifiedAgent = "Service";
            model._LastModifiedBy = this.Username;
            model._DeletedAgent = "Service";
            model._DeletedBy = this.Username;
        }

        public TokenSignViewModel TokenSign(string Username)
        {
            Account account =
                this.DbSet
                    .Include(p => p.AccountProfile)
                    .Include(p => p.AccountRoles)
                        .ThenInclude(p => p.Role)
                            .ThenInclude(p => p.Permissions)
                    .Single(p => p._IsDeleted.Equals(false) && p.Username.Equals(Username));

            TokenSignViewModel data = new TokenSignViewModel();
            data.permission = new Dictionary<string, object>();
            data.profile = new TokenSignProfileViewModel();
            data.username = account.Username;
            data.profile.firstname = account.AccountProfile.Firstname;
            data.profile.lastname = account.AccountProfile.Lastname;
            data.profile.gender = account.AccountProfile.Gender;

            foreach (AccountRole accountRole in account.AccountRoles)
            {
                foreach (Permission permission in accountRole.Role.Permissions)
                {
                    if(!data.permission.ContainsKey(permission.UnitCode))
                        data.permission.Add(permission.UnitCode, permission.permission);
                }
            }

            return data;
        }
    }
}
