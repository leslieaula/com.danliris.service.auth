using Com.DanLiris.Service.Auth.Lib.Services;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Text;
using IdentityServer4.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using Com.DanLiris.Service.Auth.Lib.ViewModels;
using Newtonsoft.Json;

namespace Com.DanLiris.Service.Auth.Lib.Authentication
{
    public class ProfileService : IProfileService
    {
        private readonly AccountService _accountService;

        public ProfileService(AccountService accountService)
        {
            _accountService = accountService;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            string username = context.Subject.Claims.ToList().Find(s => s.Type == "sub").Value;
            TokenSignViewModel tokenSign = _accountService.TokenSign(username);

            var claims = new List<Claim>
            {
                new Claim("name", tokenSign.username),
                new Claim("username", tokenSign.username),
                new Claim("profile", JsonConvert.SerializeObject(tokenSign.profile)),
                new Claim("permission", JsonConvert.SerializeObject(tokenSign.permission))
            };

            context.IssuedClaims.AddRange(claims);
            return Task.FromResult(0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.FromResult(0);
        }
    }
}
