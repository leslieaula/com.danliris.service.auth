using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Com.DanLiris.Service.Auth.Lib.Services;
using Com.DanLiris.Service.Auth.WebApi.Helpers;
using System.Linq;
using Newtonsoft.Json;

namespace Com.DanLiris.Service.Auth.WebApi.Controllers.v1
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/me")]
    [Authorize]
    public class MeController : Controller
    {
        private readonly AccountService _service;
        private static readonly string ApiVersion = "1.0";

        public MeController(AccountService accountService)
        {
            this._service = accountService;
        }

        [HttpGet]
        public IActionResult Me()
        {
            try
            {
                var Claims = User.Claims.ToList();
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("username", Claims.Find(c => c.Type.Equals("username")).Value);
                data.Add("profile", JsonConvert.DeserializeObject(Claims.Find(c => c.Type.Equals("profile")).Value));
                data.Add("permission", JsonConvert.DeserializeObject(Claims.Find(c => c.Type.Equals("permission")).Value));

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(data);
                return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
    }
}