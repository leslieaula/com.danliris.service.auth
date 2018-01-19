using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Com.DanLiris.Service.Auth.WebApi.Helpers;
using Com.DanLiris.Service.Auth.Lib.Services;
using Com.DanLiris.Service.Auth.Lib.Models;
using Com.DanLiris.Service.Auth.Lib.ViewModels;
using Com.DanLiris.Service.Auth.Lib;

namespace Com.DanLiris.Service.Auth.WebApi.Controllers.v1
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/accounts")]
    [Authorize]
    public class AccountsController : BasicController<AccountService, Account, AccountViewModel, AuthDbContext>
    {
        private static readonly string ApiVersion = "1.0";

        public AccountsController(AccountService service) : base(service, ApiVersion)
        {
            
        }
    }
}