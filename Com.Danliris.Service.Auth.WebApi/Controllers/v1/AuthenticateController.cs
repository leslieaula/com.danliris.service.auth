using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Com.DanLiris.Service.Auth.Lib.Services;
using Com.DanLiris.Service.Auth.Lib.ViewModels;
using Com.DanLiris.Service.Auth.WebApi.Helpers;

namespace Com.DanLiris.Service.Auth.WebApi.Controllers.v1
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/authenticate")]
    public class AuthenticateController : Controller
    {
        private static readonly string ApiVersion = "1.0";
        private readonly AccountService accountService;

        public AuthenticateController(AccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LoginViewModel User)
        {
            try
            {
                string token = await this.accountService.Authenticate(User.Username, User.Password);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok<string>(token);

                return Ok(Result);
            }
            catch (Exception ex)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, ex.Message)
                    .Fail();

                return BadRequest(Result);
            }
        }
    }
}