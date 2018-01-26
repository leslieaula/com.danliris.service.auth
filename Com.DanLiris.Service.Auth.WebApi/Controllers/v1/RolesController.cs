using Microsoft.AspNetCore.Mvc;
using Com.DanLiris.Service.Auth.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Com.DanLiris.Service.Auth.Lib;
using Com.DanLiris.Service.Auth.Lib.ViewModels;
using Com.DanLiris.Service.Auth.Lib.Models;
using Com.DanLiris.Service.Auth.Lib.Services;

namespace Com.DanLiris.Service.Auth.WebApi.Controllers.v1
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/roles")]
    [Authorize]
    public class RolesController : BasicController<RoleService, Role, RoleViewModel, AuthDbContext>
    {
        private static readonly string ApiVersion = "1.0";

        public RolesController(RoleService service) : base(service, ApiVersion)
        {

        }
    }
}