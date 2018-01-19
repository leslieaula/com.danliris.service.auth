using Com.DanLiris.Service.Auth.Lib.Models;
using Com.DanLiris.Service.Auth.Lib.Services;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Auth.Lib.Authentication
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly AccountService accountService;

        public ResourceOwnerPasswordValidator(AccountService accountService)
        {
            this.accountService = accountService;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                var user = await accountService.DbSet.SingleOrDefaultAsync(p => p.Username.Equals(context.UserName));

                if (user != null)
                {
                    if (user.Password == context.Password)
                    {
                        context.Result = new GrantValidationResult(
                            subject: user.Username.ToString(),
                            authenticationMethod: "Grant"
                        );

                        return;
                    }

                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Incorrect password");
                    return;
                }
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "User does not exist");
                return;
            }
            catch (Exception ex)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password");
            }
        }
    }
}
